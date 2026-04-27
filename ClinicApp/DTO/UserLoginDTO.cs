using System.ComponentModel.DataAnnotations;

namespace ClinicApp.DTO
{
    public record UserLoginDTO
    {

        [Required(ErrorMessage = "{0} field required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "length 2-50")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "{0} field required")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$", ErrorMessage = "password not meeting requirements")]
        public string? Password { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
