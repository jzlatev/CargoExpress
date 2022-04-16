namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class TruckService : ITruckService
    {
        private readonly IApplicationDbRepository repo;

        public TruckService(IApplicationDbRepository _repo)
        {
            this.repo = _repo;
        }

        public (IEnumerable<TruckAllViewModel>, int totalTrucks) All(string searchTerm, int currentPage)
        {
            var truckQuery = repo.All<Truck>()
                .Include(d => d.Driver)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                truckQuery = truckQuery.Where(t =>
                    t.PlateNumber.ToLower().Contains(searchTerm.ToLower()) ||
                    (t.Driver.EGN).Contains(searchTerm.ToLower()));
            }

            var totalTrucks = truckQuery.Count();

            var allTrucks = truckQuery
                .Skip((currentPage - 1) * TruckSearchQueryModel.TrucksPerPage)
                .Take(TruckSearchQueryModel.TrucksPerPage)
                .Select(t => new TruckAllViewModel
                {
                    Id = t.Id,
                    PlateNumber = t.PlateNumber,
                    DriverId = t.DriverId,
                    DriverName = t.Driver.FirstName + ' ' + t.Driver.LastName,
                    IsBusy = t.IsBusy == true ? "Yes" : "No",
                })
                .ToList();

            return (allTrucks, totalTrucks);
        }

        public Task Create(TruckCreateViewModel model)
        {
            Truck truck = new Truck
            {
                PlateNumber = model.PlateNumber,
                IsBusy = model.IsBusy,
                DriverId = model.DriverId
            };

            if (repo.All<Truck>().Any(t => t.DriverId == model.DriverId) && model.DriverId != null)
            {
                throw new FormException("DriverId", "The driver is busy.");
            }


            if (repo.All<Truck>().Any(d => d.PlateNumber == model.PlateNumber))
            {
                throw new FormException("PlateNumber", "The truck exists.");
            }

            try
            {
                repo.AddAsync(truck);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            { }

            return Task.CompletedTask;
        }

        public void Delete(Guid guid)
        {
            Truck? truck = repo.All<Truck>()
                .Where(t => t.Id == guid)
                .FirstOrDefault();

            Delivery? delivery = repo.All<Delivery>()
                .Where(d => d.TruckId == truck.DeliveryId)
                .FirstOrDefault();

            if (delivery != null)
            {
                delivery.TruckId = null;
                delivery.PickedAt = null;
                delivery.DeliveredAt = null;
            }

            if (truck != null)
            {
                repo.Delete(truck);
                repo.SaveChanges();
            }
        }

        public void Edit(Guid guid, TruckCreateViewModel model)
        {
            Truck? truck = repo.All<Truck>()
                .Where(t => t.Id == guid)
                .ToList().FirstOrDefault();

            if (truck == null)
            {
                throw new Exception();
            }

            if (repo.All<Truck>().Any(t => (t.DriverId == model.DriverId && t.Id != guid)) && model.DriverId != null)
            {
                throw new FormException("DriverId", "The driver is busy.");
            }


            if (repo.All<Truck>().Any(d => (d.PlateNumber == model.PlateNumber && d.Id != guid)))
            {
                throw new FormException("PlateNumber", "The truck exists.");
            }

            truck.PlateNumber = model.PlateNumber;
            truck.DriverId = model.DriverId;

            repo.SaveChanges();
        }

        public TruckCreateViewModel? GetTruckViewModelByGuid(Guid guid)
        {
            var truckList = repo.All<Truck>()
                .Where(t => t.Id == guid)
                .Select(t => new TruckCreateViewModel
                {
                    PlateNumber = t.PlateNumber,
                    DriverId = t.DriverId,
                })
                .ToList();

            if (truckList.Count == 0)
            {
                return null;
            }

            return truckList.First();
        }

        public void PopulateAvailableDrivers(TruckCreateViewModel model)
        {
            var drivers = repo.All<Driver>().Select(d => d).ToList();
            Dictionary<string, string> availableDrivers = new Dictionary<string, string>();

            foreach (var driver in drivers)
            {
                availableDrivers.Add(driver.Id.ToString(), String.Concat(driver.FirstName, " ", driver.LastName));
            }

            model.AvailableDrivers = availableDrivers;
        }
    }


}
