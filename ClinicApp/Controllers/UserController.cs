using ClinicApp.DTO;
using ClinicApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApp.Controllers
{
    public class UserController : Controller
    {

        private readonly IApplicationService _applicationService;
        private readonly ILogger<UserController> _logger;

        public UserController(IApplicationService applicationService, ILogger<UserController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {

            if (User.IsInRole("ADMIN"))
            {
                return RedirectToAction("Index", "Admin");

            }
            else if (User.IsInRole("DOCTOR"))
            {
                return RedirectToAction("Index", "Doctor");
            }
            else if (User.IsInRole("PATIENT"))
            {
                return RedirectToAction("Index", "Patient");
            }
            else
            {
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            ClaimsPrincipal? principal = HttpContext.User;

            if (!principal.Identity!.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("Index", "Doctor");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO credentials)
        {
            if (!ModelState.IsValid)
            {
                return View(credentials);
            }

            try
            {
                var user = await _applicationService.UserService.VerifyAndGetUserAsync(credentials);

                if(user is null)
                {
                    ViewData["ValidateMessage"] = "Bad Credentials. Username or password is invalid";
                    return View(credentials);
                }

                var principal = _applicationService.UserService.CreateClaimsPrincipal(user);

                AuthenticationProperties properties = new()
                {
                    AllowRefresh = true,
                    IsPersistent = credentials.KeepLoggedIn
                };


                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    properties);

                return RedirectToAction("Index", "Doctor");

            } catch (Exception e)
            {
                ViewData["ValidateMessage"] = e.Message;
                return View(credentials);
            }

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity!.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User {Username} logged out successfully", username);
            return RedirectToAction("Login", "User");
        }
    }
}
