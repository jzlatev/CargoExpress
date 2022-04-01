namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using System.Threading.Tasks;

    public class WarehouseService : IWarehouseService
    {
        private readonly IApplicationDbRepository repo;

        public WarehouseService(IApplicationDbRepository _repo)
        {
            this.repo = _repo;
        }

        public Task Create(WarehouseCreateViewModel model)
        {
            var isExist = repo.All<Warehouse>().Any(w => w.WarehouseCode == model.WarehouseCode && w.Name == model.Name);

            if (isExist)
            {
                throw new InvalidOperationException("The warehouse exists.");
            }

            Warehouse warehouse = new Warehouse
            {
                WarehouseCode = model.WarehouseCode,
                Name = model.Name,
                Address = model.Address
            };

            try
            {
                repo.AddAsync(warehouse);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            { }

            return Task.CompletedTask;
        }
    }
}
