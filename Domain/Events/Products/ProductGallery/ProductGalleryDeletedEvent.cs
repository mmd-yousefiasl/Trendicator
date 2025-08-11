using MediatR;

namespace Domain.Events.Products.ProductGallery
{
    public record ProductGalleryDeletedEvent(
        int Id
        ) : INotification;
}
