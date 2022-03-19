using System.ComponentModel.DataAnnotations;

namespace CargoExpress.Infrastructure.Data.Models
{
    public class Warehouse
    {
        [Key]
        public Guid Id { get; set; } = new Guid();

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(300)]
        public string? Address { get; set; }

        public virtual IList<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}
