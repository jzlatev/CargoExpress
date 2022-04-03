namespace CargoExpress.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TruckSearchQueryModel
    {
        public const int TrucksPerPage = 10;

        [Display(Name = "Search by plate number or drivers EGN")]
        public string SearchTerm { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalTrucks { get; set; }

        public IEnumerable<TruckAllViewModel> Trucks { get; set; }
    }
}
