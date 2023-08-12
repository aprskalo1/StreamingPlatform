using Microsoft.AspNetCore.Mvc;

namespace RWAProject.Controllers
{
    public class AccessDeniedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
