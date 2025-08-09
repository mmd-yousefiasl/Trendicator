using Application.Services.Interfaces;
using Application.ViewModels.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class FooterSiteDataViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public FooterSiteDataViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetSiteData();
            if (result.IsFailure) return View("FooterSiteData", new ClientSideSiteDataViewModel());

            return View("FooterSiteData", result.Value);
        }
    }
}
