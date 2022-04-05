using CargoExpress.Core.Contracts;
using CargoExpress.Core.Exceptions;
using CargoExpress.Core.Models;
using CargoExpress.Core.Models.Enums;
using CargoExpress.Infrastructure.Data.Models;
using CargoExpress.Infrastructure.Data.Repositories;

namespace CargoExpress.Core.Services
{
    public class CargoService : ICargoService
    {
        private readonly IApplicationDbRepository repo;

        public CargoService(IApplicationDbRepository _repo)
        {
            this.repo = _repo;
        }

        public (IEnumerable<CargoAllViewModel>, int totalCargo) All(string? searchTerm, CargoSorting sorting, int currentPage)
        {
            var cargoQuery = repo.All<Cargo>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cargoQuery = cargoQuery.Where(c => 
                c.Name.ToLower().Contains(searchTerm.ToLower()) ||
                c.Description.ToLower().Contains(searchTerm.ToLower()) ||
                c.CargoRef.ToString().ToLower().Contains(searchTerm.ToLower()));
            }

            cargoQuery = sorting switch
            {
                CargoSorting.Date => cargoQuery.OrderByDescending(d => d.CreatedAt),
                CargoSorting.Name => cargoQuery.OrderByDescending(n => n.Name),
                CargoSorting.Weight => cargoQuery.OrderByDescending(w => w.Weight),
                CargoSorting.Type => cargoQuery.OrderByDescending(t => t.IsDangerous),
                _ => cargoQuery.OrderByDescending(r => r.CreatedAt)
            };

            var totalNumCargo = cargoQuery.Count();

            var allCargosCurrentPage = cargoQuery
                .Skip((currentPage - 1) * CargoSearchQueryModel.CargosPerPage) // Curnt page - 1 * number of all pages
                .Take(CargoSearchQueryModel.CargosPerPage)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CargoAllViewModel
                {
                    Id = c.Id.ToString(),
                    CargoRef = c.CargoRef,
                    Name = c.Name,
                    Weight = c.Weight,
                    CreatedAt = c.CreatedAt,
                    IsDangerous = c.IsDangerous == true ? "Yes" : "No",
                    Description = c.Description,
                    Status = c.Delivery != null ? "Picked" : "Free",
                })
                .ToList();

            return (allCargosCurrentPage, totalNumCargo);
        }

        public Task Create(CargoCreateViewModel model)
        {
            Cargo cargo = new Cargo
            {
                Name = model.Name,
                Weight = model.Weight,
                Description = model.Description,
                IsDangerous = model.IsDangerous
            };

            var cargoRef = cargo.CargoRef;

            if (repo.All<Cargo>().Any(c => c.CargoRef == cargoRef) && cargoRef != null)
            {
                throw new FormException(nameof(cargo.CargoRef), "The delivery exists.");
            }

            try
            {
                repo.AddAsync(cargo);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            { }

            return Task.CompletedTask;
        }

        public void Delete(Guid guid)
        {
            Cargo? cargo = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .ToList()
                .FirstOrDefault();

            if (cargo != null)
            {
                repo.Delete(cargo);
                repo.SaveChanges();
            }
        }

        public void Edit(Guid guid, CargoCreateViewModel model)
        {
            Cargo? cargo = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .ToList()
                .FirstOrDefault();

            if (cargo == null)
            {
                throw new Exception();
            }

            cargo.Name = model.Name;
            cargo.Weight = model.Weight;
            cargo.Description = model.Description;
            cargo.IsDangerous = model.IsDangerous;

            repo.SaveChanges();
        }

        public CargoCreateViewModel? GetCargoViewModelByGuid(Guid guid)
        {
            var cargoList = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .Select(c => new CargoCreateViewModel
                {
                    Name = c.Name,
                    Weight = c.Weight,
                    Description = c.Description,
                    IsDangerous = c.IsDangerous
                })
                .ToList();

            if (cargoList.Count == 0)
            {
                return null;
            }

            return cargoList.First();
        }
    }
}
