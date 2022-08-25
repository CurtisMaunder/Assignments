using AdminWebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminWebPortal.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password) {
            if (username != "admin" || password != "admin") {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again");
                return View();
            }

            //Login admin
            HttpContext.Session.SetString("Admin", "Admin");

            return RedirectToAction("Index", "AdminPanel");
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}