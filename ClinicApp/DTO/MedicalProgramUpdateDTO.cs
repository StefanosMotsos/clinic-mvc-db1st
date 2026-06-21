using System.ComponentModel.DataAnnotations;

namespace ClinicApp.DTO
{
    public record MedicalProgramUpdateDTO
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        public string? Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "Required")]
        [StringLength(255, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "StringLength")]
        public string? Description { get; set; }
    }
}
