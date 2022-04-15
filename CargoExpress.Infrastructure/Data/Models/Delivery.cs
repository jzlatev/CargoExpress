using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Delivery
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(38)]
        public string? DeliveryRef { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? PickedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [ForeignKey(nameof(Warehouse))]
        [MaxLength(36)]
        public Guid? PickWarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [MaxLength(500)]
        public string? PickAddress { get; set; }

        [MaxLength(36)]
        public Guid? DeliveryWarehouseId { get; set; }

        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }

        [MaxLength(36)]
        public Guid? TruckId { get; set; }

        [Required]
        [MaxLength(36)]
        public string? UserId { get; set; }

        public virtual IList<Cargo> Cargos { get; set; } = new List<Cargo>();

        public virtual IList<Truck> Trucks { get; set; } = new List<Truck>();

        public string getStatus()
        {
            if (DeliveredAt == null)
            {
                if (PickedAt == null)
                {
                    return "Pending";
                }
                else
                {
                    return "In progress";
                }
            }

            return "Done";
        }
    }
}