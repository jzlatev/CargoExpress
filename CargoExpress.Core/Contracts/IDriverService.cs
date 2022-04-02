namespace CargoExpress.Core.Contracts
{
    using CargoExpress.Core.Models;

    public interface IDriverService
    {
        Task Create(DriverCreateViewModel model);
    }
}
