namespace CargoExpress.Core.Models
{
    using System.ComponentModel.DataAnnotations;

    public class WarehouseSearchQueryModel
    {
        public const int WarehousesPerPage = 10;

        [Display(Name = "Search by word or text")]
        public string SearchTerm { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalWarehouses { get; set; }

        public IEnumerable<WarehouseAllViewModel> Warehouses { get; set;}
    }
}
