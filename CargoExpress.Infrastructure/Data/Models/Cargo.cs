using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Cargo
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
        [MaxLength(20)]
        public string? CargoRef { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [Range(1, 300000)]
        public float Weight { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0, 1)]
        public bool IsDangerous { get; set; }

        [ForeignKey(nameof(Delivery))]
        public Guid? DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
    }
}
