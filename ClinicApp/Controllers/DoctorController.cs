using AutoMapper;
using ClinicApp.Core;
using ClinicApp.Core.Filters;
using ClinicApp.DTO;
using ClinicApp.Exceptions;
using ClinicApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicApp.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IMapper _mapper;
        public List<Error> ErrorArray { get; set; } = [];

        public DoctorController(IApplicationService applicationService, IMapper mapper)
        {
            _applicationService = applicationService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, DoctorFiltersDTO? filters = null)
        {
            try
            {
                filters ??= new DoctorFiltersDTO();
                PaginatedResult<DoctorReadOnlyDTO> result = await _applicationService.DoctorService
                    .GetPaginatedFilteredDoctorsAsync(pageNumber, pageSize, filters);
                return View(result);
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View(new PaginatedResult<DoctorReadOnlyDTO>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,EMPLOYEE,DOCTOR")]
        public async Task<IActionResult> Details(Guid uuid)
        {
            try
            {
                DoctorReadOnlyDTO doctor = await _applicationService.DoctorService.GetDoctorByUuidAsync(uuid);
                return View(doctor);
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return RedirectToAction("Index", "Doctor");
            }
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Signup(DoctorSignupDTO doctorSignupDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(doctorSignupDTO);
            }

            try
            {
                await _applicationService.DoctorService.SaveDoctorAsync(doctorSignupDTO);
                return RedirectToAction("Index", "Doctor");
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View(doctorSignupDTO);
            }
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,DOCTOR")]
        public async Task<IActionResult> Edit()
        {
            try
            {
                Guid uuid = Guid.Parse(User.FindFirstValue("uuid")!);
                DoctorReadOnlyDTO doctor = await _applicationService.DoctorService.GetDoctorByUuidAsync(uuid);
                DoctorUpdateDTO dto = _mapper.Map<DoctorUpdateDTO>(doctor);
                return View(dto);
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View(new DoctorUpdateDTO());
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,DOCTOR")]
        public async Task<IActionResult> Edit(DoctorUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                await _applicationService.DoctorService.UpdateDoctorAsync(dto, userId);
                return RedirectToAction("Index", "Doctor");
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return View(dto);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(Guid uuid)
        {
            try
            {
                await _applicationService.DoctorService.DeleteDoctorAsync(uuid);
                return RedirectToAction("Index", "Doctor");
            }
            catch (Exception e)
            {
                ErrorArray.Add(new Error("", e.Message, ""));
                ViewData["ErrorArray"] = ErrorArray;
                return RedirectToAction("Index", "Doctor");
            }
        }
    }
}
