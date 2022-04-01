namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Models.Enums;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DeliveryService : IDeliveryService
	{
		private readonly IApplicationDbRepository repo;

		public DeliveryService(IApplicationDbRepository _repo)
		{
			this.repo = _repo;
		}

        public (IEnumerable<DeliveryAllViewModel>, int totalDeliveries) All(string searchTerm, DeliverySorting deliverySorting, int currentPage)
        {
			var deliveryQuery = repo.All<Delivery>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchTerm))
            {
				deliveryQuery = deliveryQuery.Where(d =>
				d.DeliveryRef.ToLower().Contains(searchTerm.ToLower()) ||
				d.DeliveryAddress.ToLower().Contains(searchTerm.ToLower()) ||
				d.PickAddress.ToLower().Contains(searchTerm.ToLower()));
            }

			deliveryQuery = deliverySorting switch
			{
				DeliverySorting.CreatedAt => deliveryQuery.OrderByDescending(c => c.CreatedAt),
				DeliverySorting.PickedAt => deliveryQuery.OrderByDescending(p => p.PickedAt),
				DeliverySorting.DeliveredAt => deliveryQuery.OrderByDescending(d => d.DeliveredAt),
				_ => deliveryQuery.OrderByDescending(d => d.CreatedAt)
			};

			var totalDeliveries = deliveryQuery.Count();

			var deliveries = deliveryQuery
				.Skip((currentPage - 1) * DeliverySearchQueryModel.DeliveriesPerPage)
				.Take(DeliverySearchQueryModel.DeliveriesPerPage)
				.OrderByDescending(d => d.CreatedAt)
				.Select(d => new DeliveryAllViewModel
				{ 
					Id = d.Id.ToString(),
					DeliveryRef = d.DeliveryRef,
					CreatedAt = d.CreatedAt.ToString(),
					PickedAt = d.PickedAt.ToString(),
					DeliveredAt = d.DeliveredAt.ToString(),
					PickWarehouseId = d.PickWarehouseId.ToString(),
					PickAddress = d.PickAddress,
					DeliveryWarehouseId	= d.DeliveryWarehouseId.ToString(),
					DeliveryAddress = d.DeliveryAddress,
				})
				.ToList();

			return (deliveries, totalDeliveries);
        }

        public async Task Create(DeliveryCreateViewModel model, ClaimsPrincipal user)
		{
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

			Delivery delivery = new Delivery
            {
				PickedAt = model.PickedAt,
				DeliveredAt = model.DeliveredAt,
				PickWarehouseId = model.PickWarehouseId,
				PickAddress = model.PickAddress,
				DeliveryWarehouseId = model.DeliveryWarehouseId,
				DeliveryAddress = model.DeliveryAddress,
				UserId = userId.ToString()
			};

			var deliveryRef = delivery.DeliveryRef;
			bool isExist = repo.All<Delivery>()
				.Any(d => d.DeliveryRef == deliveryRef);
			if (isExist)
            {
				throw new InvalidOperationException("The delivery exists.");
            }

			try
            {
                await repo.AddAsync(delivery);
                repo.SaveChanges();
        }
			catch (InvalidOperationException)
			{ }
}
	}
}
