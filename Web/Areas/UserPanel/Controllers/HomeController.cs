using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.UserPanel.Controllers
{
    public class HomeController : BaseUserPanelController
    {
        [HttpGet("/user/home")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
