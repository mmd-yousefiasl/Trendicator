using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

        protected async Task<string> RenderViewAsync(string viewName, object model, bool partial = false)
        {
            ViewData.Model = model;

            using var writer = new StringWriter();
            var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            var viewResult = partial
                ? viewEngine.FindView(ControllerContext, viewName, false)
                : viewEngine.GetView(null, viewName, false);

            if (!viewResult.Success)
            {
                throw new FileNotFoundException($"View '{viewName}' not found.");
            }

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }
}
