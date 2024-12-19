using CareerManagementApp.DAL.Context;
using CareerManagementApp.MODEL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using System.Security.Claims;

namespace CareerManagementApp.WEB.Controllers
{
    public class HomeController : Controller
    {
        public MyDbContext context { get; set; }
        private readonly GoogleAI _googleAI;

        public HomeController(MyDbContext _context, GoogleAI googleAI)
        {
            context = _context;
            _googleAI = googleAI;
        }

        public IActionResult Index()
        {
            List<Blog> list = context.Blogs
                .OrderByDescending(b => b.CreatedDate)
                .Take(4)
                .ToList();
            return View(list);
        }

        public IActionResult Blog()
        {
            List<Blog> list = context.Blogs.ToList();
            return View(list);
        }

        public IActionResult BlogContent(Guid id)
        {
            Blog blog = context.Blogs.Include(b => b.User).FirstOrDefault(c => c.ID == id);
            return View(blog);
        }

        [HttpGet]
        public IActionResult VisualCareerAdvisor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VisualCareerAdvisor(MessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
            {
                ModelState.AddModelError("UserMessage", "Mesaj bo� olamaz.");
                return View();
            }
            request.UserMessage += ": NOT! Sen bir kariyer dan��manl��� uzman�s�n. Rol�n yaln�zca kariyerle ilgili sorular� yan�tlamak ve konuyla ilgisiz sorular� g�rmezden gelmek. E�er gelen soru kariyerle ilgili de�ilse, kibar bir �ekilde �u yan�t� ver: \"Yaln�zca kariyerle ilgili sorular� yan�tlayabilirim.\" Kariyer planlama, i� arama, beceri geli�tirme veya i�yeriyle ilgili konularda d���nceli, profesyonel ve uygulanabilir tavsiyeler sun. Yan�tlar�n� her zaman k�sa ve kariyer konusuna uygun �ekilde tut. Mesleklerle ilgili sorulara cevap verebilirsin ama kibar olmaya da �zen g�ster kaba sorular gelirse cevaplama.";
            var model = _googleAI.GenerativeModel(Model.GeminiPro);
            var response = await model.GenerateContent(request.UserMessage);


            ViewBag.ResponseText = FormatResponse(response.Text);

            AIChat chat = new AIChat();
            chat.UserID = new Guid();
            chat.Message = request.UserMessage;
            chat.AIResponse = response.Text;
            context.AIChats.Add(chat);
            context.SaveChanges();
            return View(); 
        }
        private string FormatResponse(string? responseText)
        {
            var formattedText = responseText
                .Replace("**", "")
                .Replace("\n", "")
                .Replace("* ", "")
                .Insert(0, "") 
                .Insert(responseText.Split('\n').Length, "</ul>"); 

            return formattedText;
        }

    }

    public class MessageRequest
    {
        public string UserMessage { get; set; }
    }
}
