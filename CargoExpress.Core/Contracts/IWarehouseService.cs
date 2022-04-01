namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;

    public interface IWarehouseService
    {
       Task Create(WarehouseCreateViewModel model);
    }
}
