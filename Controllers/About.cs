using Microsoft.AspNetCore.Mvc;

namespace MVCDemo.Controllers
{
    public class About : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
