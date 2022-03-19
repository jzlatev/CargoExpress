using System.ComponentModel.DataAnnotations;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Driver
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

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
        [MinLength(10)]
        [MaxLength(10)]
        public int EGN { get; set; }

        public Truck? Truck { get; set; }
    }
}
