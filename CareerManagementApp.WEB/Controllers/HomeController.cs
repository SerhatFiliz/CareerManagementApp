using CareerManagementApp.DAL.Context;
using CareerManagementApp.MODEL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            var claims = HttpContext.User.Claims;
            User user = new User();
            if (claims!=null)
            {
                var email = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                user = context.Users.FirstOrDefault(x => x.Email == email);
            }
            ViewBag.user = user;

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
                ModelState.AddModelError("UserMessage", "Mesaj boþ olamaz.");
                return View();
            }
            request.UserMessage += ": NOT! Sen bir kariyer danýþmanlýðý uzmanýsýn. Rolün yalnýzca kariyerle ilgili sorularý yanýtlamak ve konuyla ilgisiz sorularý görmezden gelmek. Eðer gelen soru kariyerle ilgili deðilse, kibar bir þekilde þu yanýtý ver: \"Yalnýzca kariyerle ilgili sorularý yanýtlayabilirim.\" Kariyer planlama, iþ arama, beceri geliþtirme veya iþyeriyle ilgili konularda düþünceli, profesyonel ve uygulanabilir tavsiyeler sun. Yanýtlarýný her zaman kýsa ve kariyer konusuna uygun þekilde tut. Mesleklerle ilgili sorulara cevap verebilirsin ama kibar olmaya da özen göster kaba sorular gelirse kaba bir soru soruþ tarzý olduðu için soruyu cevaplamayacaðýný belirt ve cevaplama.";
            var model = _googleAI.GenerativeModel(Model.GeminiPro);
            var response = await model.GenerateContent(request.UserMessage);


            ViewBag.ResponseText = FormatResponse(response.Text);

            AIChat chat = new AIChat();
            chat.UserID = Guid.Parse("4A04E449-C512-4F8F-9F04-C90938922521");
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

        public IActionResult Profile()
        {
            var claims = HttpContext.User.Claims;
            User user = new User();
            if (claims != null)
            {
                var email = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                user = context.Users.FirstOrDefault(x => x.Email == email);
            }   

            List<Blog> blogs = new List<Blog>();
            blogs = context.Blogs.Include(y=>y.User).Where(x => x.UserID == user.ID).ToList();
            ViewBag.blog = blogs;
            ViewBag.user = user;
            return View();
        }
        public IActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBlog(Blog blog, string action)
        {
            if (ModelState.IsValid)
            {
                var claims = HttpContext.User.Claims;
                User user = new User();
                if (claims != null)
                {
                    var email = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
                    user = context.Users.FirstOrDefault(x => x.Email == email);
                }

                Blog saveBlog = new Blog();
                saveBlog.CreatedDate = DateTime.Now;
                saveBlog.Status = action == "Publish" ? "Publish" : "Draft";
                saveBlog.Content = blog.Content;
                saveBlog.Title = blog.Title;
                saveBlog.ID = Guid.NewGuid();
                saveBlog.UserID = user.ID;

                // Veritabanýna kaydetme iþlemi
                context.Blogs.Add(saveBlog);
                context.SaveChanges();

                return RedirectToAction("Profile", "User");
            }

            // Eðer model doðrulanmazsa formu geri döndür
            return View(blog);
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            User user = new User();
            user.ID = Guid.NewGuid();
            user.Name = claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            user.Email = claims?.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            user.RoleID = Guid.Parse("23504766-0928-4AA6-B9D5-08A2A126E30E");

            User checkUser = context.Users.FirstOrDefault(x => x.Email == user.Email);

            if (checkUser == null)
            {
                context.Users.Add(user);
                context.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }

    public class MessageRequest
    {
        public string UserMessage { get; set; }
    }
}
