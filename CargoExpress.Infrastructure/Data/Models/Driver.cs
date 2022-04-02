using System.ComponentModel.DataAnnotations;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Driver
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(35)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(35)]
        public string? MiddleName { get; set; }

        [Required]
        [MaxLength(35)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(10)]
        public string? EGN { get; set; }

        public Truck? Truck { get; set; }
    }
}
