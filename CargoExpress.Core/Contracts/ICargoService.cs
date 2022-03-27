using CargoExpress.Core.Models;
using CargoExpress.Core.Models.Enums;

namespace CargoExpress.Core.Contracts
{
    public interface ICargoService
    {
        Task Create(CargoCreateViewModel model);

        public IEnumerable<CargoAllViewModel> All(string searchTerm, CargoSorting sorting);
    }
}
