namespace CargoExpress.Core.Models
{
	using System.ComponentModel.DataAnnotations;
    using CargoExpress.Core.CustomAttributes;

    /// <summary>
    /// Create delivery.
    /// </summary>
    public class DeliveryCreateViewModel
    {
        /// <summary>
        /// Date of pick cargo
        /// </summary>
        [Required]
        [Display(Name = "Date of pick cargo")]
        public DateTime? PickedAt { get; set; }

        /// <summary>
        /// Date of delivery cargo
        /// </summary>
        [Required]
        [Display(Name = "Date of delivery cargo")]
        [IsBefore(nameof(PickedAt), errorMessage: "Date must be after date of pick!")]
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// Warehouse of cargo
        /// </summary>
        [MinLength(36, ErrorMessage = "The minimum length must be 36 symbols!")]
        [MaxLength(36, ErrorMessage = "The maximum length must be 36 symbols!")]
        [Display(Name = "Loading warehouse number")]
        public Guid? PickWarehouseId { get; set; }

        /// <summary>
        /// Loading address
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "Loading address")]
        public string? PickAddress { get; set; }

        /// <summary>
        /// Delivery warehouse
        /// </summary>=
        [MinLength(36, ErrorMessage = "The minimum length must be 36 symbols!")]
        [MaxLength(36, ErrorMessage = "The maximum length must be 36 symbols!")]
        [Display(Name = "Delivery warehouse number")]
        public Guid? DeliveryWarehouseId { get; set; }

        /// <summary>
        /// Delivery address
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "Delivery address")]
        public string? DeliveryAddress { get; set; }
    }
}
