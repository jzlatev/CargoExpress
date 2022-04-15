using CargoExpress.Core.Models;
using CargoExpress.Core.Models.Enums;
using System.Security.Claims;

namespace CargoExpress.Core.Contracts
{
    public interface ICargoService
    {
        Task Create(CargoCreateViewModel model, ClaimsPrincipal user);

        public (IEnumerable<CargoAllViewModel>, int totalCargo) All(string? searchTerm, CargoSorting sorting, int currentPage, ClaimsPrincipal user);

        CargoCreateViewModel? GetCargoViewModelByGuid(Guid guid);

        void Edit(Guid guid, CargoCreateViewModel model);

        void Delete(Guid guid);
    }
}
