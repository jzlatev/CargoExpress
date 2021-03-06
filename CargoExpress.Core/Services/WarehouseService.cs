namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
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
                    Id = w.Id,
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
            {
                throw new InvalidOperationException("A problem has occurred. Contact your administrator.");
            }

            return Task.CompletedTask;
        }

        public void Delete(Guid guid)
        {
            Warehouse? warehouse = repo.All<Warehouse>()
                .Where(w => w.Id == guid)
                .FirstOrDefault();

            if (warehouse == null)
            {
                throw new Exception();
            }

            repo.Delete(warehouse);
            repo.SaveChanges();
        }

        public void Edit(Guid guid, WarehouseCreateViewModel model)
        {
            Warehouse? warehouse = repo.All<Warehouse>()
                .Where(w => w.Id == guid)
                .FirstOrDefault();

            if (warehouse == null)
            {
                throw new Exception();
            }

            if (repo.All<Warehouse>().Any(d => (d.WarehouseCode == model.WarehouseCode && d.Id != guid)))
            {
                throw new FormException(nameof(model.WarehouseCode), "The warehouse code exists.");
            }

            warehouse.WarehouseCode = model.WarehouseCode;
            warehouse.Name = model.Name;
            warehouse.Address = model.Address;

            repo.SaveChanges();
        }

        public WarehouseCreateViewModel? GetWarehouseViewModelByGuid(Guid guid)
        {
            var warehouse = repo.All<Warehouse>()
                .Where(w => w.Id == guid)
                .Select(w => new WarehouseCreateViewModel
                {
                    WarehouseCode = w.WarehouseCode,
                    Name = w.Name,
                    Address = w.Address
                })
                .ToList();

            if (warehouse.Count == 0)
            {
                return null;
            }

            return warehouse.First();
        }
    }
}
