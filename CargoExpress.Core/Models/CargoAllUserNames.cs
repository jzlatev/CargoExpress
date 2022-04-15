namespace CargoExpress.Core.Models
{
    public class CargoAllUserNames
    {
        public string Guid { get; set; }

        public string? FirstName{ get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string getFullname()
        {
            return $"{FirstName} {LastName} - {Email}";
        }
    }
}
