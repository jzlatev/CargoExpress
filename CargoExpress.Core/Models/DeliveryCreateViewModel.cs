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
        [Display(Name = "Date of pick cargo")]
        public DateTime? PickedAt { get; set; }

        /// <summary>
        /// Date of delivery cargo
        /// </summary>
        [Display(Name = "Date of delivery cargo")]
        //[IsBefore(nameof(PickedAt), errorMessage: "Date must be after date of pick!")]
        public DateTime? DeliveredAt { get; set; }

        [Required]
        [Display(Name = "Cargo name")]
        public Guid? CargoId { get; set; }

        public Dictionary<string, string>? CargoNames { get; set; }

        /// <summary>
        /// Warehouse of cargo
        /// </summary>
        [Required]
        [Display(Name = "Loading warehouse number")]
        public Guid? PickWarehouseId { get; set; }

        public Dictionary<string, string>? WarehouseNames { get; set; }

        /// <summary>
        /// Loading address
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "Loading address")]
        public string? PickAddress { get; set; }

        /// <summary>
        /// Delivery warehouse
        /// </summary>
        [Display(Name = "Delivery warehouse number")]
        public Guid? DeliveryWarehouseId { get; set; }

        public Dictionary<string, string>? DeliveryWHouseNames { get; set; }

        /// <summary>
        /// Delivery address
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "Delivery address")]
        public string? DeliveryAddress { get; set; }

        [Display(Name = "Truck plate number")]
        public Guid? TruckId { get; set; }

        public Dictionary<string, string>? TruckPlates { get; set; }
    }
}
