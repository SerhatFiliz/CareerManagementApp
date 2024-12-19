using CareerManagementApp.DAL.Context;
using CareerManagementApp.MODEL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return View(); // GET isteði için boþ bir view döndür
        }

        [HttpPost]
        public async Task<IActionResult> VisualCareerAdvisor(MessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
            {
                ModelState.AddModelError("UserMessage", "Mesaj boþ olamaz.");
                return View(); // Hata durumunda ayný view'a geri dön
            }

            var model = _googleAI.GenerativeModel(Model.GeminiPro); // Modeli seçin
            var response = await model.GenerateContent(request.UserMessage);

            // Yanýtý ViewBag ile taþýyoruz
            ViewBag.ResponseText = FormatResponse(response.Text);
            return View(); // Yanýtý göstermek için ayný view'a dönüyoruz
        }

        // Yanýtý daha okunabilir bir formatta düzenleyen yardýmcý metot
        private string FormatResponse(string? responseText)
        {
            // Yanýt metnindeki özel karakterleri ve biçimlendirmeleri düzenleyin
            var formattedText = responseText
                .Replace("**", "") // Kalýn yazýyý kaldýr
                .Replace("\n", "<li>") // Yeni satýrlarý liste öðesi yap
                .Replace("* ", "<li>") // Yýldýzlarý liste öðesi yap
                .Insert(0, "<ul>") // Liste baþlangýcý
                .Insert(responseText.Split('\n').Length, "</ul>"); // Liste sonu

            return formattedText;
        }

        public IActionResult Profile()
        {
            return View();
        }

    }

    public class MessageRequest
    {
        public string UserMessage { get; set; } // Özellik adýný deðiþtirdik
    }
}
