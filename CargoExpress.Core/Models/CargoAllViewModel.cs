namespace CargoExpress.Core.Models
{
    public class CargoAllViewModel
    {
        public string? Id { get; set; }

        public Guid? CargoRef { get; set; }

        public string? Name { get; set; }

        public float Weight { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Description { get; set; }

        public string? IsDangerous { get; set; }

        public string? Status { get; set; }
    }
}
