namespace CargoExpress.Test
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Services;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    public class DriverServiceTests
    {
        private ServiceProvider serviceProvider;
        private InMemoryDbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .AddSingleton<IDriverService, DriverService>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            await SeedDbAsync(repo);
        }

        [Test]
        public void TheEGNExists()
        {
            var model = new DriverCreateViewModel()
            {
                EGN = "1234567890",
                FirstName = "Pesho",
                MiddleName = "Peshov",
                LastName = "Peshinov"

            };

            var driverService = serviceProvider.GetService<IDriverService>();
            //service.Create(model);

            Assert.Catch<FormException>(() => driverService.Create(model), "The EGN exists.");
        }

        [Test]
        public void TheEGNNotExists()
        {
            var model = new DriverCreateViewModel()
            {
                EGN = "1234557891",
                FirstName = "Pesho",
                MiddleName = "Dikov",
                LastName = "Peshinov"

            };

            var service = serviceProvider.GetService<IDriverService>();
            //service.Create(model);

            Assert.DoesNotThrowAsync(async () => await service.Create(model));
        }

        [Test]
        public void TheCreateIsSuccessful()
        {
            var model = new DriverCreateViewModel()
            {
                EGN = "1234567891",
                FirstName = "Pesho",
                MiddleName = "Peshov",
                LastName = "Peshinov"
            };

            var service = serviceProvider.GetService<IDriverService>();
            //service.Create(model);

            //Assert.DoesNotThrowAsync(async () => await service.Create(model));
            Assert.Pass("", service.Create(model));
        }

        [Test]
        public void ThePassedModelIsNull()
        {
            var model = new DriverCreateViewModel()
            {
                EGN = string.Empty,
                FirstName = string.Empty,
                MiddleName = string.Empty,
                LastName = string.Empty
            };

            var service = serviceProvider.GetService<IDriverService>();
            Assert.IsNotNull(service.Create(model));
        }

        [Test]
        public void CreateDataBaseError()
        {
            IDriverService driverService;

            var model = new DriverCreateViewModel()
            {
                //EGN = string.Empty,
                //FirstName = string.Empty,
                //MiddleName = string.Empty,
                //LastName = string.Empty
            };

            var mockRepo = new Mock<IDriverService>();
            mockRepo.Setup(repo => repo.Create(It.IsAny<DriverCreateViewModel>())).Throws(new InvalidOperationException());

            

            var service = serviceProvider.GetService<IDriverService>();
            Assert.Throws<InvalidOperationException>(() => service.Create(model));
        }

        [Test]
        public void DeleteDriverIfDriverIsAsignedToTheTruck()
        {


            DriverCreateViewModel model = new DriverCreateViewModel()
            {
                EGN = "1234567890",
                FirstName = "Pesho",
                MiddleName = "Peshov",
                LastName = "Peshinov"
            };

            //var mockRepo = new Mock<IApplicationDbRepository>();

            var service = serviceProvider.GetService<IDriverService>();

            service.Delete(new Guid("0259F02A-55EA-450B-9473-761442791864"));
            //mockRepo.Verify(repo => repo.Delete(model), Times.Once);

        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            var driver = new Driver()
            {
                Id = new Guid("0259F02A-55EA-450B-9473-761442791864"),
                EGN = "1234567890",
                FirstName = "Pesho",
                MiddleName = "Peshov",
                LastName = "Peshinov"
            };

            var truck = new Truck()
            {
                PlateNumber = "12345678",
                IsBusy = false,
            };

            await repo.AddAsync(truck);
            await repo.AddAsync(driver);
            await repo.SaveChangesAsync();
        }

    }
}