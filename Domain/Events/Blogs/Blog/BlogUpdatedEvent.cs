using MediatR;

namespace Domain.Events.Blogs.Blog
{
    public record BlogUpdatedEvent(
    int id,
    string BannerFileName,
    string Title,
    string Slug,
    string Body,
    string Writer,
    int Priority,
    bool IsPublished,
    int BlogCategoryId,
    int ReadingTimeInMinutes
    ) : INotification;
}
