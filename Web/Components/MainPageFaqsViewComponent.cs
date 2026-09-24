using Application.Dtos.Faq.ClientSide;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Components;

public class MainPageFaqsViewComponent : ViewComponent
{
    private readonly IFaqService _faqService;

    public MainPageFaqsViewComponent(IFaqService faqService)
    {
        _faqService = faqService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int take)
    {
        var result = await _faqService.ClientSideGetFaqsForMainPage(take);
        if (result.IsFailure)
            return View("MainPageFaqs", new List<ClientSideFaqViewModel>());

        return View("MainPageFaqs", result.Value);
    }
}