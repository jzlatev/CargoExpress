using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Delivery
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
        [MaxLength(20)]
        public string? DeliveryRef { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now.Date;

        public DateTime? PickedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [ForeignKey(nameof(Warehouse))]
        public Guid? PickWarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        [MaxLength(500)]
        public string? PickAddress { get; set; }

        public Guid? DeliveryWarehouseId { get; set; }

        [MaxLength(500)]
        public string? DeliveryAddress { get; set; }

        public Guid? TruckId { get; set; }

        [Required]
        public string? UserId { get; set; }

        public virtual IList<Cargo> Cargos { get; set; } = new List<Cargo>();

        public virtual IList<DeliveryTruck> DeliveriesTrucks { get; set; } = new List<DeliveryTruck>();
    }
}