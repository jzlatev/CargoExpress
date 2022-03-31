namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
	using CargoExpress.Core.Models;
	using CargoExpress.Infrastructure.Data.Models;
	using CargoExpress.Infrastructure.Data.Repositories;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.IdentityModel.Tokens;
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
                CreatedAt = DateTime.Now.Date,
				//PickWarehouseId = model.PickWarehouseId,
				PickAddress = model.PickAddress,
				//DeliveryWarehouseId = model.DeliveryWarehouseId,
				DeliveryAddress = model.DeliveryAddress,
				//UserId = userId,
				Cargos = new List<Cargo>(),
				DeliveriesTrucks = new List<DeliveryTruck>()
			};

            //try
            //{
                await repo.AddAsync(delivery);
                await repo.SaveChangesAsync();
			//}
			//catch (InvalidOperationException)
			//{ }
		}
	}
}
