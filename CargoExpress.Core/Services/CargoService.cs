using CargoExpress.Core.Contracts;
using CargoExpress.Core.Models;
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

        public IEnumerable<CargoAllViewModel> All()
        {
           var allCargos =  repo.All<Cargo>()
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CargoAllViewModel
                {
                    Id = c.Id.ToString(),
                    CargoRef = c.CargoRef,
                    Name = c.Name,
                    Weight = c.Weight,
                    CreatedAt = c.CreatedAt,
                    IsDangerous = c.IsDangerous == true ? "Yes" : "No",
                    Description = c.Description
                })
                .ToList();

            return allCargos;
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
                DeliveryId = model.DeliveryId
            };

            try
            {
                await repo.AddAsync(cargo);
                await repo.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {}
        }
    }
}
