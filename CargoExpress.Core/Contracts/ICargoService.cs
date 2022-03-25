using CargoExpress.Core.Models;

namespace CargoExpress.Core.Contracts
{
    public interface ICargoService
    {
        Task Create(CargoCreateViewModel model);

        public IEnumerable<CargoAllViewModel> All();
    }
}
