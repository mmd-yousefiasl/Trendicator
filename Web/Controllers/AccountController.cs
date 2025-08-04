using Application.Services.Interfaces;
using Application.ViewModels.User.ClientSide;
using Domain.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    public class AccountController : BaseController
    {
        #region Fields
        private readonly IUserService _userService;
        #endregion

        #region Ctor
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        [HttpGet("/login")]
        public IActionResult Login(string ReturnUrl)
        {
            if (User?.Identity?.IsAuthenticated ?? false) return Redirect("/");
            return View(new ClientSideLoginViewModel() { ReturnUrl = ReturnUrl });
        }

        [HttpPost("/login"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ClientSideLoginViewModel model)
        {
            if (User?.Identity?.IsAuthenticated ?? false) return Redirect("/");
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.SendUserLoginCode(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View(model);
            }
            TempData[TemporarySuccessMessage] = result.Message;

            return RedirectToAction(nameof(LoginCode), new
            {
                PhoneNumber = model.PhoneNumber,
                ReturnUrl = model.ReturnUrl
            });
        }

        [HttpGet("/login-code")]
        public IActionResult LoginCode(ClientSideLoginCodeViewModel model)
        {
            if (model is null)
            {
                TempData[TemporaryErrorMessage] = Messages.LoginFailedError;
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }

        [HttpPost("/login-code")]
        public async Task<IActionResult> VerifyLoginCode(ClientSideLoginCodeViewModel model)
        {
            if (!ModelState.IsValid) return View("LoginCode", model);

            var result = await _userService.VerifyLoginCode(model);
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return View("LoginCode", model);
            }

            await LoginUser(result.Value);

            TempData[TemporarySuccessMessage] = Messages.LogedInSuccessfully;
            if (model.ReturnUrl is null)
            {
                return Redirect("/");
            }
            else
            {
                return Redirect(model.ReturnUrl);
            }
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            if (!User?.Identity?.IsAuthenticated ?? true) return Redirect("/");

            TempData[TemporarySuccessMessage] = Messages.LogedoutSuccessfully;
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        private async Task LoginUser(ClientSideUserViewModel user, bool rememberMe = true)
        {
            var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Mobile),
                    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTime.Now.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
    }
}
