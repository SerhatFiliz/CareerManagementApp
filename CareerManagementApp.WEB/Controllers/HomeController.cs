using CareerManagementApp.MODEL;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CareerManagementApp.WEB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult index() { 
        
            return View();
        
        }
    }
}
