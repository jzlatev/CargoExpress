namespace CargoExpress.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DriverSearchQueryModel
    {
        public const int DriversPerPage = 10;

        [Display(Name = "Search by word or text")]
        public string SearchTerm { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalDrivers { get; set; }

        public IEnumerable<DriverAllViewModel> Drivers { get; set; }
    }
}
