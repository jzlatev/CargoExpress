using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Truck
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        public string? PlateNumber { get; set; }

        [ForeignKey(nameof(Driver))]
        public Guid? DriverId { get; set; }
        public Driver? Driver { get; set; }

        [Required]
        public bool IsBusy { get; set; }

        [ForeignKey(nameof(Delivery))]
        public Guid? DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
    }
}
