namespace CargoExpress.Core.Services
{
    using System.Threading.Tasks;
    using CargoExpress.Core.Contracts;
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

        public Task Create(DriverCreateViewModel model)
        {
            Driver driver = new Driver
            {
                EGN = model.EGN,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName
                
            };

            bool egnExist = repo.All<Driver>().Any(d => d.EGN == model.EGN);
            if (egnExist)
            {
                throw new InvalidOperationException("The EGN exists.");
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
    }
}
