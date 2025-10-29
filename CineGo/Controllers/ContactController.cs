using Microsoft.AspNetCore.Mvc;

namespace CineGo.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View("Contact");
        }
    }
}
