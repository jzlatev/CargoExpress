using CargoExpress.Core.Contracts;
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
                c.CargoRef.ToLower().Contains(searchTerm.ToLower()));
            }

            cargoQuery = sorting switch
            {
                CargoSorting.Date => cargoQuery.OrderByDescending(d => d.CreatedAt),
                CargoSorting.Name => cargoQuery.OrderByDescending(n => n.Name),
                CargoSorting.Weight => cargoQuery.OrderByDescending(w => w.Weight),
                CargoSorting.Type => cargoQuery.OrderByDescending(t => t.IsDangerous),
                _ => cargoQuery.OrderByDescending(r => r.CargoRef)
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

        public async Task Create(CargoCreateViewModel model)
        {
            Cargo cargo = new Cargo
            {
                CargoRef = model.CargoRef,
                Name = model.Name,
                Weight = model.Weight,
                Description = model.Description,
                IsDangerous = model.IsDangerous,
                CreatedAt = DateTime.Now
            };

            try
            {
                await repo.AddAsync(cargo);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            {}
        }
    }
}
