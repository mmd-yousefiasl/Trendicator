using MediatR;

namespace Application.Mediator
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T @event) where T : INotification;
    }
}
