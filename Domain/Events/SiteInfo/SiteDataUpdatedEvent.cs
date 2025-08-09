using MediatR;

namespace Domain.Events.SiteInfo
{
    public record SiteDataUpdatedEvent(
    string? DarkBgLogo,
    string? LightBgLogo,
    string? Phone,
    string? FooterDescription,
    string? Address,
    string? Email,
    string? TermsAndCondition,
    string? PrivacyPolicy
    ) : INotification;
}
