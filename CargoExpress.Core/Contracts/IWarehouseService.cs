namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;

    public interface IWarehouseService
    {
        Task Create(WarehouseCreateViewModel model);

        public (IEnumerable<WarehouseAllViewModel>, int totalWarehouses) All(string searchTerm, int currentPage);
    }
}
