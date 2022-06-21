using Microsoft.AspNetCore.Mvc;

namespace a_RegistrationApp.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Back()
        {
            return View("Index");
        }
    }
}
