using MediatR;

namespace Domain.Events.Products.ProductGallery
{
    public record ProductGalleryCreatedEvent(
        string FileName
        ) : INotification;
}
