using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        public static string SuccessMessage = "SuccessMessage";
        public static string ErrorMessage = "ErrorMessage";
        public static string InfoMessage = "InfoMessage";
        public static string WarningMessage = "WarningMessage";
        public static string TemporaryErrorMessage = "TemporaryErrorMessage";
        public static string TemporarySuccessMessage = "TemporarySuccessMessage";
        public static string TemporaryMessage = "TemporaryMessage";
    }
}