using MediatR;

namespace Domain.Events.Products.ProductCategory
{
    public record ProductCategoryUpdatedEvent(
        int Id,
        string Title,
        string Slug,
        string MetaDescription,
        int Priority,
        bool IsPublished,
        int? ParentCategoryId
        ) : INotification;
}
