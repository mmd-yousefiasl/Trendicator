using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class CheckoutController : BaseController
    {
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
