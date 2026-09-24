using Application.Dtos.NewsLetter.ClientSide;
using Application.Services.Interfaces;
using Application.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Xml.Linq;

namespace Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Fields
        private readonly INewsLetterService _newsLettersService;
        private readonly ISiteMapService _mapService;
        #endregion

        #region Ctor
        public HomeController(INewsLetterService newsLettersService,
            ISiteMapService mapService)
        {
            _newsLettersService = newsLettersService;
            _mapService = mapService;
        }
        #endregion

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/sitemap.xml")]
        public async Task<IActionResult> SiteMap()
        {
            var result = await _mapService.ClientSideGetSiteMap();
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var maps = result.Value; // IEnumerable<ClientSideSiteMapViewModel>

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var xDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    maps.Select(m =>
                        new XElement(ns + "url",
                            new XElement(ns + "loc", m.Url),
                            new XElement(ns + "lastmod", m.LastUpdateDate.ToString("yyyy-MM-dd")),
                            new XElement(ns + "changefreq", m.ChangeFreq.GetDisplayName()),
                            new XElement(ns + "priority", m.Priority.ToString(CultureInfo.InvariantCulture))
                        )
                    )
                )
            );

            return Content(xDoc.ToString(), "application/xml");
        }

        [HttpPost("/sign-in-news-letters"), ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInNewsLetters(ClientSideSignInNewsLettersViewModel model)
        {
            if (!ModelState.IsValid) return View(nameof(Index), model);

            var result = await _newsLettersService.SignInNewsLetters(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(nameof(Index), model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [HttpGet("/403")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("/500")]
        public IActionResult InternalServerError()
        {
            return View();
        }
    }
}
