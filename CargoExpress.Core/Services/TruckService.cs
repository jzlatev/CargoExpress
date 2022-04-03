﻿namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
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

            bool isExist = repo.All<Truck>().Any(d => d.PlateNumber == model.PlateNumber);

            if (isExist)
            {
                throw new InvalidOperationException("The truck exists.");
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
    }
}
