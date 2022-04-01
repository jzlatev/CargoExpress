namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class WarehouseService : IWarehouseService
    {
        private readonly IApplicationDbRepository repo;

        public WarehouseService(IApplicationDbRepository _repo)
        {
            this.repo = _repo;
        }

        public (IEnumerable<WarehouseAllViewModel>, int totalWarehouses) All(string searchTerm, int currentPage)
        {
            var warehouseQuery = repo.All<Warehouse>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                warehouseQuery = warehouseQuery.Where(w =>
                    w.WarehouseCode.ToLower().Contains(searchTerm.ToLower()) ||
                    w.Name.ToLower().Contains(searchTerm.ToLower()) ||
                    w.Address.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalWarehouses = warehouseQuery.Count();

            var allWarehouses = warehouseQuery
                .Skip((currentPage - 1) * WarehouseSearchQueryModel.WarehousesPerPage)
                .Take(WarehouseSearchQueryModel.WarehousesPerPage)
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseAllViewModel
                {
                    WarehouseCode = w.WarehouseCode,
                    Name = w.Name,
                    Address = w.Address
                })
                .ToList();

            return (allWarehouses, totalWarehouses);
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
