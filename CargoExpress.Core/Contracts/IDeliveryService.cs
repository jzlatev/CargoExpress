namespace CargoExpress.Core.Contracts
{
	using CargoExpress.Core.Models;
    using CargoExpress.Core.Models.Enums;
    using CargoExpress.Infrastructure.Data.Models;
    using System;
    using System.Security.Claims;

	public interface IDeliveryService
    {
        Task Create(DeliveryCreateViewModel model, ClaimsPrincipal user);

        public (IEnumerable<DeliveryAllViewModel>, int totalDeliveries) All(string searchTerm, DeliverySorting deliverySorting, int currentPage, ClaimsPrincipal user);

        void PopulateWarehouse(DeliveryCreateViewModel model);
        
        void PopulateTruck(DeliveryCreateViewModel model);

        DeliveryCreateViewModel? GetDeliveryViewModelByGuid(Guid guid);

        void Edit(Guid guid, DeliveryCreateViewModel model, ClaimsPrincipal user);

        void Delete(Guid guid);

        void Pick(Guid guid);

        void Deliver(Guid guid);
        Delivery? GetDeliveryByGuid(Guid guid);
    }
}
