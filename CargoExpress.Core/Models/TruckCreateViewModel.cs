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

        [Display(Name = "Driver")]
        public Guid? DriverId { get; set; }

        public Dictionary<string, string>? AvailableDrivers { get; set; }
   
        [Required]
        [Display(Name = "Is busy")]
        public bool IsBusy { get; set; }
    }
}
