using ClinicApp.Core;
using ClinicApp.DTO;
using ClinicApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IApplicationService _applicationService;
        public List<Error> ErrorArray { get; set; } = [];

        public DoctorController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        [Authorize(Roles = "DOCTOR")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup(DoctorSignupDTO doctorSignupDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(doctorSignupDTO);
            }

            try
            {
                await _applicationService.DoctorService.SignUpUserAsync(doctorSignupDTO);

                return RedirectToAction("Login", "User");
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View(doctorSignupDTO);
            }
        }
    }
}
