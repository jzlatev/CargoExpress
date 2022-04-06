namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;
    using System;

    public interface IDriverService
    {
        Task Create(DriverCreateViewModel model);

        public (IEnumerable<DriverAllViewModel>, int totalDrivers) All(string searchTerm, int currentPage);
        
        DriverCreateViewModel? GetDriverViewModelByGuid(Guid guid);

        void Edit(Guid guid, DriverCreateViewModel model);

        void Delete(Guid guid);
    }
}
