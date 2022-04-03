namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;

    public interface ITruckService
    {
        Task Create(TruckCreateViewModel model);

        public (IEnumerable<TruckAllViewModel>, int totalTrucks) All(string searchTerm, int currentPage);
    }
}
