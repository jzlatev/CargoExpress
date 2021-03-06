using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Cargo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(36)]
        public Guid? CargoRef { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [Range(1, 300000)]
        public float Weight { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public bool IsDangerous { get; set; }

        [Required]
        [MaxLength(36)]
        public string? UserId { get; set; }

        [ForeignKey(nameof(Delivery))]
        public Guid? DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }

        public string getStatus()
        {
            if (Delivery == null)
            {
                return "Not assigned to a delivery";
            }

            if (Delivery.DeliveredAt == null)
            {
                if (Delivery.PickedAt == null)
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
