using System.ComponentModel.DataAnnotations;

namespace ClinicApp.DTO
{
    public record DoctorSignupDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "length 2-50")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$", ErrorMessage = "password not meeting requirements")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "length up to 50")]
        [EmailAddress(ErrorMessage = "invalid address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "length 2-50")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "length 2-50")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int? RoleId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "length 2-100")]
        public string? Specialty { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "length 10-50")]
        public string? phoneNumber { get; set; }
    }
}
