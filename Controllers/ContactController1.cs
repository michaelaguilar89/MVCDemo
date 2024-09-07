using Microsoft.AspNetCore.Mvc;

namespace MVCDemo.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
