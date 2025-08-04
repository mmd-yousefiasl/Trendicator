using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        [HttpGet("/admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
