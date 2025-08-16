using MediatR;

namespace Domain.Events.Products.ProductReview
{
    public record ProductReviewDeletedEvent(
        int id
        ) : INotification;
}
