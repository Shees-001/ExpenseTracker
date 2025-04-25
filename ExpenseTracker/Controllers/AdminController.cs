using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
