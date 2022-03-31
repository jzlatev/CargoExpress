namespace CargoExpress.Core.Models
{
	using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Create delivery.
    /// </summary>
    public class DeliveryCreateViewModel
    {
        /// <summary>
        /// Warehouse of cargo
        /// </summary>
        [MaxLength(36)]
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
        /// </summary>
        [MaxLength(36)]
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
