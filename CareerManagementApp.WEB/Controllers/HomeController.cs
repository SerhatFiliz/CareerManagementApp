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
            return View(); // GET iste�i i�in bo� bir view d�nd�r
        }

        [HttpPost]
        public async Task<IActionResult> VisualCareerAdvisor(MessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
            {
                ModelState.AddModelError("UserMessage", "Mesaj bo� olamaz.");
                return View(); // Hata durumunda ayn� view'a geri d�n
            }

            var model = _googleAI.GenerativeModel(Model.GeminiPro); // Modeli se�in
            var response = await model.GenerateContent(request.UserMessage);

            // Yan�t� ViewBag ile ta��yoruz
            ViewBag.ResponseText = FormatResponse(response.Text);
            return View(); // Yan�t� g�stermek i�in ayn� view'a d�n�yoruz
        }

        // Yan�t� daha okunabilir bir formatta d�zenleyen yard�mc� metot
        private string FormatResponse(string? responseText)
        {
            // Yan�t metnindeki �zel karakterleri ve bi�imlendirmeleri d�zenleyin
            var formattedText = responseText
                .Replace("**", "") // Kal�n yaz�y� kald�r
                .Replace("\n", "<li>") // Yeni sat�rlar� liste ��esi yap
                .Replace("* ", "<li>") // Y�ld�zlar� liste ��esi yap
                .Insert(0, "<ul>") // Liste ba�lang�c�
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
        public string UserMessage { get; set; } // �zellik ad�n� de�i�tirdik
    }
}
