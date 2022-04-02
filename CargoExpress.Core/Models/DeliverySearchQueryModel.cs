namespace CargoExpress.Core.Models
{
    using CargoExpress.Core.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    public class DeliverySearchQueryModel
    {
        public const int DeliveriesPerPage = 10;

        [Display(Name = "Search by word or text")]
        public string SearchTerm { get; set; }

        [Display(Name = "Sorting by")]
        public DeliverySorting Sorting { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TotalDeliveries { get; set; }

        public IEnumerable<DeliveryAllViewModel>? Deliveries { get; set; }
    }
}
