namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;
    using System;

    public interface IWarehouseService
    {
        Task Create(WarehouseCreateViewModel model);

        public (IEnumerable<WarehouseAllViewModel>, int totalWarehouses) All(string searchTerm, int currentPage);

        WarehouseCreateViewModel? GetWarehouseViewModelByGuid(Guid guid);

        void Edit(Guid guid, WarehouseCreateViewModel model);
    }
}
