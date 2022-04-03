namespace CargoExpress.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TruckCreateViewModel
    {
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        [Display(Name = "Plate number")]
        public string? PlateNumber { get; set; }

        [Required]
        [Display(Name = "Driver")]
        public Guid? DriverId { get; set; }

        [Required]
        [Display(Name = "Is busy")]
        public bool IsBusy { get; set; }
    }
}
