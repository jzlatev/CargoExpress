namespace CargoExpress.Core.Models
{
    using CargoExpress.Core.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class CargoSearchQueryModel
    {
        public const int CargosPerPage = 10;

        [Display(Name = "Search by word or text")]
        public string? SearchTerm { get; set; }

        public CargoSorting Sorting { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalCargo { get; set; }

        public IEnumerable<CargoAllViewModel> Cargos { get; set; }
    }
}
