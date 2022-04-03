namespace CargoExpress.Core.Models
{
    public class TruckAllViewModel
    {
        public string? PlateNumber { get; set; }

        public Guid? DriverId { get; set; }

        public string? DriverName { get; set; }

        public string? IsBusy { get; set; }
    }
}
