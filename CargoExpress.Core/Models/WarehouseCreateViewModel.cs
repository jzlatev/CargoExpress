namespace CargoExpress.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class WarehouseCreateViewModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(10)]
        [Display(Name = "Warehouse code")]
        public string? WarehouseCode { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(300)]
        public string? Address { get; set; }
    }
}
