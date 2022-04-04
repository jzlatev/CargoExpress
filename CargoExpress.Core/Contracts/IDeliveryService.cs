namespace CargoExpress.Core.Contracts
{
	using CargoExpress.Core.Models;
    using CargoExpress.Core.Models.Enums;
    using System.Security.Claims;

	public interface IDeliveryService
    {
        Task Create(DeliveryCreateViewModel model, ClaimsPrincipal user);

        public (IEnumerable<DeliveryAllViewModel>, int totalDeliveries) All(string searchTerm, DeliverySorting deliverySorting, int currentPage);

        void PopulateWarehouse(DeliveryCreateViewModel model);
    }
}
