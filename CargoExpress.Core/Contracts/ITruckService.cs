namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;

    public interface ITruckService
    {
        Task Create(TruckCreateViewModel model);

        public (IEnumerable<TruckAllViewModel>, int totalTrucks) All(string searchTerm, int currentPage);

        void PopulateAvailableDrivers(TruckCreateViewModel model);

        TruckCreateViewModel? GetTruckViewModelByGuid(Guid guid);

        void Edit(Guid guid, TruckCreateViewModel model);

        void Delete(Guid guid);
    }
}
