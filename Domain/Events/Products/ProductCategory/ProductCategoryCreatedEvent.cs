using MediatR;

namespace Domain.Events.Products.ProductCategory
{
    public record ProductCategoryCreatedEvent(
        string Title,
        string Slug,
        string MetaDescription,
        int Priority,
        bool IsPublished,
        int? ParentCategoryId
        ) : INotification;
}
