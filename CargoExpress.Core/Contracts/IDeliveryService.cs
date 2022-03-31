namespace CargoExpress.Core.Contracts
{
	using CargoExpress.Core.Models;
	using System.Security.Claims;

	public interface IDeliveryService
    {
        Task Create(DeliveryCreateViewModel model, ClaimsPrincipal user);
    }
}
