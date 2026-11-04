using Application.Services.Interfaces;
using Domain.Events.Users;
using MediatR;

namespace Application.Handlers.Users
{
    public class UserCartHandlers : INotificationHandler<UserLoggedInEvent>
    {
        #region Fields
        private readonly ICartService _cartService;
        #endregion

        #region Ctor
        public UserCartHandlers(ICartService cartService)
        {
            _cartService = cartService;
        }
        #endregion

        public async Task Handle(UserLoggedInEvent notification, CancellationToken cancellationToken)
        {
            await _cartService.ClientSideUpdateCartAfterUserLoggedIn(notification.Id);
        }
    }
}
