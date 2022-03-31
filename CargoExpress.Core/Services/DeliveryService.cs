namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DeliveryService : IDeliveryService
	{
		private readonly IApplicationDbRepository repo;

		public DeliveryService(IApplicationDbRepository _repo)
		{
			this.repo = _repo;
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
