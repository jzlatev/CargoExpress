namespace CargoExpress.Infrastructure.Data.Models
{
    public class DeliveryTruck
    {
        public Guid DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }

        public Guid TruckId { get; set; }
        public Truck? Truck { get; set; }
    }
}
