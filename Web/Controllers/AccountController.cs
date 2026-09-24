using Application.Dtos.User.ClientSide;
using Application.Services.Interfaces;
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
        public IActionResult Login(string ReturnUrl, string? Mobile)
        {
            ViewData["NoIndex"] = true;
            if (User?.Identity?.IsAuthenticated ?? false) return Redirect("/");
            return View(new ClientSideLoginDto() { ReturnUrl = ReturnUrl });
        }

        [HttpPost("/login"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(ClientSideLoginDto model)
        {
            if (User?.Identity?.IsAuthenticated ?? false) return Redirect("/");
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.SendUserLoginCode(model);
            if (result.IsFailure)
            {
                TempData[TemporaryMessage] = result.Message;
                return View(model);
            }
            string formattedDate = result.Value.ToString("o");

            return RedirectToAction(nameof(LoginCode), new
            {
                Mobile = model.Mobile,
                ReturnUrl = model.ReturnUrl,
                CreatedDate = formattedDate
            });
        }

        [HttpGet("/login-code")]
        public IActionResult LoginCode(ClientSideLoginCodeDto model)
        {
            ViewData["NoIndex"] = true;
            if (model is null)
            {
                TempData[TemporaryErrorMessage] = Messages.LoginFailedError;
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        [HttpPost("/login-code")]
        public async Task<IActionResult> VerifyLoginCode(ClientSideLoginCodeDto model)
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

        [HttpGet("/resend-code")]
        public async Task<IActionResult> ResendCode(string ReturnUrl, string Mobile)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
                return Redirect("/");

            if (string.IsNullOrWhiteSpace(Mobile))
            {
                TempData[TemporaryErrorMessage] = Messages.OperationFailedError;
                return RedirectToAction(nameof(Login));
            }

            var result = await _userService.SendUserLoginCode(new ClientSideLoginDto()
            {
                Mobile = Mobile,
                ReturnUrl = ReturnUrl,
                PrivacyPolicy = true,
            });
            if (result.IsFailure)
            {
                TempData[TemporaryErrorMessage] = result.Message;
                return RedirectToAction(nameof(Login));
            }
            TempData[TemporarySuccessMessage] = Messages.LoginCodeSentSuccessfully;
            string formattedDate = result.Value.ToString("o");

            return RedirectToAction(nameof(LoginCode), new
            {
                Mobile = Mobile,
                ReturnUrl = ReturnUrl,
                CreatedDate = formattedDate
            });
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout()
        {
            if (!User?.Identity?.IsAuthenticated ?? true) return Redirect("/");

            TempData[TemporarySuccessMessage] = Messages.LogedoutSuccessfully;
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        private async Task LoginUser(ClientSideUserDto user, bool rememberMe = true)
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
                IsPersistent = rememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
    }
}
