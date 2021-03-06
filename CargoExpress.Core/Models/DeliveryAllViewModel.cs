namespace CargoExpress.Core.Models
{
    using CargoExpress.Infrastructure.Data.Models;

    public class DeliveryAllViewModel
    {
        public string? Id { get; set; }

        public string? DeliveryRef { get; set; }

        //public string? CargoRef { get; set; }

        //public string? CargoName { get; set; }

        //public float CargoWeight { get; set; }

        public string? CreatedAt { get; set; }

        public string? PickedAt { get; set; }

        public string? DeliveredAt { get; set; }

        public string? PickWarehouseId { get; set; }

        public string? PickWarehouseName { get; set; }

        public string? PickAddress { get; set; }

        public string? DeliveryWarehouseId { get; set; }

        public string? DeliveryWarehouseName { get; set; }

        public string? DeliveryAddress { get; set; }

        public string Status { get; set; }

        public IEnumerable<Cargo> Cargos { get; set; }

        public Guid? TruckId { get; set; }
    }
}
