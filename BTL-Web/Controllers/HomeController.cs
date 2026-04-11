using BTL_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BTL_Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Auth");

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "NhanVien" => RedirectToAction("Index", "Staff"),
                "GiaoVien" => RedirectToAction("Index", "Teacher"),
                "HocVien" => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Login", "Auth")
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
