namespace CargoExpress.Test
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Services;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TruckServiceTests
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
                .AddSingleton<ITruckService, TruckService>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            await SeedDbAsync(repo);
        }

        [Test]
        public void ShouldPassWithoutSearchTermInAllMethod()
        {
            var service = serviceProvider.GetService<ITruckService>();

            Assert.DoesNotThrow(() => service.All("", 1));
        }

        [Test]
        public void ShouldPassWithSearchTermInAllMethod()
        {
            var service = serviceProvider.GetService<ITruckService>();

            Assert.DoesNotThrow(() => service.All("a", 1));
        }

        [Test]
        public void ShouldReturnCorrectResultForAllMethod()
        {
            var service = serviceProvider.GetService<ITruckService>();

            var res = service.All(String.Empty, 1);

            var trucksList = new List<TruckAllViewModel>();
            var truck = new TruckAllViewModel();
            truck.Id = new Guid("0259F02A-55EA-450B-9473-761442791864");
            truck.PlateNumber = "12345678";
            truck.DriverId = new Guid("0259F02A-55EA-410B-9473-761442791864");

            trucksList.Add(truck);

            var expected = ((IEnumerable<TruckAllViewModel>) trucksList, 1);

            var expectedTruck = expected.Item1.First();
            var actualTruck = res.Item1.First();

            Assert.AreEqual(expectedTruck.Id.ToString(), actualTruck.Id.ToString());
            Assert.AreEqual(expectedTruck.PlateNumber, actualTruck.PlateNumber);
            Assert.AreEqual(expectedTruck.DriverId, actualTruck.DriverId);
        }

        [Test]
        public void ShouldThrowIfTruckExists()
        {
            var model = new TruckCreateViewModel()
            {
                PlateNumber = "12345678",
                DriverId = new Guid("0259F02A-55EA-410B-9473-761442791864"),
                IsBusy = false

            };

            var truckService = serviceProvider.GetService<ITruckService>();

            Assert.Catch<FormException>(() => truckService.Create(model), "The truck exists.");
        }

        [Test]
        public void ShouldNotThrowIfTruckNotExists()
        {
            var model = new TruckCreateViewModel()
            {
                PlateNumber = "12365678",
                IsBusy = false

            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.DoesNotThrowAsync(async () => await service.Create(model));
        }

        [Test]
        public void ShouldThrowIfTruckDesireDriverIsBusy()
        {
            var model = new TruckCreateViewModel()
            {
                PlateNumber = "12365678",
                DriverId = new Guid("0259F02A-55EA-410B-9473-761442791864")

            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.Catch<FormException>(() => service.Create(model), "The driver is busy.");
        }

        [Test]
        public void TheCreateTruckIsSuccessful()
        {

            var model = new TruckCreateViewModel()
            {
                PlateNumber = "12315178",
            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.DoesNotThrowAsync(async () => await service.Create(model));
        }

        [Test]
        public void ThePassedCreateModelIsNull()
        {
            var truck = new TruckCreateViewModel()
            {
                PlateNumber = String.Empty
            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.IsNotNull(service.Create(truck));
        }

        [Test]
        public void DeleteTruckIfIsAsignedToDelivery()
        {
            var service = serviceProvider.GetService<ITruckService>();

            Assert.DoesNotThrow(() => service.Delete(new Guid("0259F02A-55EA-450B-9473-761442791864")));
        }

        [Test]
        public void ShouldThrowExceptionIfEditTruckIsNull()
        {
            TruckCreateViewModel model = new TruckCreateViewModel()
            { };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.Catch<Exception>(() => service.Edit(new Guid("0259F02A-55EA-450B-9473-761442778864"), model));
        }

        [Test]
        public void ShouldThrowExceptionIfEditDriverIdExistAndPassedIdIsDifferent()
        {
            TruckCreateViewModel model = new TruckCreateViewModel()
            {
                PlateNumber = "12345678",
                DriverId = new Guid("0259F02A-55EA-450B-9473-761442791864")
            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.Catch<Exception>(() => service.Edit(new Guid("0259F02A-55EA-450B-9473-761122791864"), model), "The driver is busy.");
        }

        [Test]
        public void ShouldThrowExceptionIfEditTruckExist()
        {
            TruckCreateViewModel model = new TruckCreateViewModel()
            {
                PlateNumber = "12345678",
                DriverId = new Guid("0259F02A-55EA-450B-9473-761442791864")
            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.Catch<Exception>(() => service.Edit(new Guid("0259F07C-55EA-450B-9473-761122791864"), model), "The truck exists.");
        }

        [Test]
        public void ShouldNotThrowExceptionIfEditTruckIsSuccessful()
        {
            TruckCreateViewModel model = new TruckCreateViewModel()
            {
                PlateNumber = "12342678",
            };

            var service = serviceProvider.GetService<ITruckService>();

            Assert.DoesNotThrow(() => service.Edit(new Guid("0259F02A-55EA-450B-9473-761442791864"), model));
        }

        [Test]
        public void ShouldReturnNullIfGetTruckViewModelByGuidIsEmpty()
        {
            var service = serviceProvider.GetService<ITruckService>();

            Assert.Null(service.GetTruckViewModelByGuid(new Guid("a259B72A-55EA-450B-9473-761433791864")));
        }

        [Test]
        public void ShouldReturnCorrectResultForGetTruckViewModelByGuid()
        {
            var service = serviceProvider.GetService<ITruckService>();

            var res = service.GetTruckViewModelByGuid(new Guid("0259F02A-55EA-450B-9473-761442791864"));

            var truckList = new List<TruckCreateViewModel>();
            var truck = new TruckCreateViewModel();
            truck.PlateNumber = "12345678";
           
            truckList.Add(truck);

            var expected = (IEnumerable<TruckCreateViewModel>)truckList;

            var expectedtruck = expected.First();
            var actualtruck = res;

            Assert.AreEqual(expectedtruck.PlateNumber, actualtruck.PlateNumber);
        }

        [Test]
        public void ShouldBeNullIfPassedModelIsEmpty()
        {
            var service = serviceProvider.GetService<ITruckService>();

            TruckCreateViewModel model = new TruckCreateViewModel()
            {
                PlateNumber = String.Empty
            };

            Assert.DoesNotThrow(() => service.PopulateAvailableDrivers(model));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            var truck = new Truck()
            {
                Id = new Guid("0259F02A-55EA-450B-9473-761442791864"),
                PlateNumber = "12345678",
                DriverId = new Guid("0259F02A-55EA-410B-9473-761442791864"),
                IsBusy = false,
                DeliveryId = new Guid("0259F02A-15EA-450B-9473-761442791864")

            };

            var driver = new Driver()
            {
                Id = new Guid("0259F02A-55EA-410B-9473-761442791864"),
                EGN = "1234567890",
                FirstName = "Pesho",
                MiddleName = "Peshov",
                LastName = "Peshinov"
            };

            var delivery = new Delivery()
            {
                Id = new Guid("0259F02A-15EA-450B-9473-761442791864"),
                DeliveryRef = "0259F02A-15EA-450B-9473-761448591824",
                PickWarehouseId = new Guid("5459F02A-55EA-450B-9473-761442791864"),
                TruckId = new Guid("0259F02A-55EA-450B-9473-761442791864"),
                UserId = "0259F02A-55EA-450B-9473-761122791864"
            };

            var warehouse = new Warehouse()
            {
                Id = new Guid("5459F02A-55EA-450B-9473-761442791864"),
                WarehouseCode = "1234567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            await repo.AddAsync(warehouse);
            await repo.AddAsync(delivery);
            await repo.AddAsync(driver);
            await repo.AddAsync(truck);
            await repo.SaveChangesAsync();
        }
    }
}
