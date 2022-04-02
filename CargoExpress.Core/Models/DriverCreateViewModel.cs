namespace CargoExpress.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DriverCreateViewModel
    {
        [Required(ErrorMessage = "First name field is required!")]
        [MinLength(3)]
        [MaxLength(35)]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Middle name field is required!")]
        [MinLength(3)]
        [MaxLength(35)]
        [Display(Name = "Middle name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name field is required!")]
        [MinLength(3)]
        [MaxLength(35)]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "EGN must contain only 10 digits.")]
        public string? EGN { get; set; }
    }
}
