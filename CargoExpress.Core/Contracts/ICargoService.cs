using CargoExpress.Core.Models;
using CargoExpress.Core.Models.Enums;

namespace CargoExpress.Core.Contracts
{
    public interface ICargoService
    {
        Task Create(CargoCreateViewModel model);

        public (IEnumerable<CargoAllViewModel>, int totalCargo) All(string? searchTerm, CargoSorting sorting, int currentPage);

        CargoCreateViewModel? GetCargoViewModelByGuid(Guid guid);

        void Edit(Guid guid, CargoCreateViewModel model);

        void Delete(Guid guid);
    }
}
