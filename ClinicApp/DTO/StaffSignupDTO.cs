using System.ComponentModel.DataAnnotations;

namespace ClinicApp.DTO
{
    public record StaffSignupDTO
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        public string? Username { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$",
            ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "RegularExpression")]
        public string? Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "EmailAddress")]
        public string? Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        public string? Firstname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        public string? Lastname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        public int? RoleId { get; set; }
    }
}
