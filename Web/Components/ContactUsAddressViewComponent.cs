using Application.Services.Interfaces;
using Application.Dtos.SiteData.ClientSide;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components
{
    public class ContactUsAddressViewComponent : ViewComponent
    {
        private readonly ISiteDataService _dataService;
        public ContactUsAddressViewComponent(ISiteDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _dataService.ClientSideGetContactAddress();
            if (result.IsFailure) return View("ContactUsAddress", new ClientSideContactUsAddressViewModel());

            return View("ContactUsAddress", result.Value);
        }
    }
}
