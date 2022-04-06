namespace CargoExpress.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;

    public class DriverService : IDriverService
    {
        private readonly IApplicationDbRepository repo;

        public DriverService(IApplicationDbRepository _repo)
        {
            this.repo = _repo;
        }

        public (IEnumerable<DriverAllViewModel>, int totalDrivers) All(string searchTerm, int currentPage)
        {
            var truckQuery = repo.All<Driver>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                truckQuery = truckQuery.Where(t =>
                    t.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                    t.MiddleName.ToLower().Contains(searchTerm.ToLower()) ||
                    t.LastName.ToLower().Contains(searchTerm.ToLower()) ||
                    t.EGN.ToLower().Contains(searchTerm.ToLower()));
            }

            var totaltrucks = truckQuery.Count();

            var allTrucks = truckQuery
                .Skip((currentPage - 1) * DriverSearchQueryModel.DriversPerPage)
                .Take(DriverSearchQueryModel.DriversPerPage)
                .OrderBy(d => d.FirstName)
                .Select(d => new DriverAllViewModel
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    MiddleName = d.MiddleName,
                    LastName = d.LastName,
                    EGN = d.EGN
                })
                .ToList();

            return (allTrucks, totaltrucks);
        }

        public Task Create(DriverCreateViewModel model)
        {
            Driver driver = new Driver
            {
                EGN = model.EGN,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName
                
            };

            if (repo.All<Driver>().Any(d => d.EGN == model.EGN))
            {
                throw new FormException(nameof(model.EGN), "The EGN exists.");
            }

            try
            {
                repo.AddAsync(driver);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            { }

            return Task.CompletedTask;
        }

        public void Edit(Guid guid, DriverCreateViewModel model)
        {
            Driver? driver = repo.All<Driver>()
                .Where(d => d.Id == guid)
                .FirstOrDefault();

            if (driver == null)
            {
                throw new Exception();
            }

            if (repo.All<Driver>().Any(d => d.EGN == model.EGN && d.Id != guid))
            {
                throw new FormException(nameof(model.EGN), "The EGN exists.");
            }

            driver.EGN = model.EGN;
            driver.FirstName = model.FirstName;
            driver.MiddleName = model.MiddleName;
            driver.LastName = model.LastName;

            repo.SaveChanges();
        }

        public DriverCreateViewModel? GetDriverViewModelByGuid(Guid guid)
        {
            var driverList = repo.All<Driver>()
                .Where(d => d.Id == guid)
                .Select(d => new DriverCreateViewModel
                { 
                    EGN = d.EGN,
                    FirstName = d.FirstName,
                    MiddleName= d.MiddleName,
                    LastName= d.LastName
                })
                .ToList();

            if (driverList.Count == 0)
            {
                return null;
            }

            return driverList.First();
        }
    }
}
