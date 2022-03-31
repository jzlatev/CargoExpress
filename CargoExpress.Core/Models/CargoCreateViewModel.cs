using System.ComponentModel.DataAnnotations;

namespace CargoExpress.Core.Models
{
    /// <summary>
    /// Create cargo.
    /// </summary>
    public class CargoCreateViewModel
    {
        /// <summary>
        /// Uniq number of cargo.
        /// </summary>
        [Required]
        [MaxLength(20)]
        [Display(Name = "Cargo reference")]
        public string? CargoRef { get; set; }

        /// <summary>
        /// Name of cargo.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Weight of cargo.
        /// </summary>
        [Required]
        [Range(1, 300000)]
        public float Weight { get; set; }

        /// <summary>
        /// Description for cargo
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Type of cargo. Dangerous or ordinary.
        /// </summary>
        [Display(Name = "Is dangerous")]
        public bool IsDangerous { get; set; }
    }
}
