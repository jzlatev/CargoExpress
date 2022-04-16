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

    public class WarehouseServiceTests
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
                .AddSingleton<IWarehouseService, WarehouseService>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            await SeedDbAsync(repo);
        }

        [Test]
        public void ShouldPassWithoutSearchTerm()
        {
            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.DoesNotThrow(() => service.All("", 1));
        }

        [Test]
        public void ShouldPassWithSearchTerm()
        {
            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.DoesNotThrow(() => service.All("a", 1));
        }

        [Test]
        public void ShouldReturnCorrectValueForAllMethod()
        {
            var service = serviceProvider.GetService<IWarehouseService>();

            var res = service.All(String.Empty, 1);

            var warehouseList = new List<WarehouseAllViewModel>();
            var warehouse = new WarehouseAllViewModel();
            warehouse.Id = new Guid("0259F02A-55EA-450B-9473-761442791864");
            warehouse.WarehouseCode = "1234567890";
            warehouse.Name = "WarehouseName";
            warehouse.Address = "AddressAddress";

            warehouseList.Add(warehouse);

            var expected = ((IEnumerable<WarehouseAllViewModel>)warehouseList, 1);

            var expectedWarehouse = expected.Item1.First();
            var actualWarehouse = res.Item1.First();

            Assert.AreEqual(expectedWarehouse.Id.ToString(), actualWarehouse.Id.ToString());
            Assert.AreEqual(expectedWarehouse.WarehouseCode, actualWarehouse.WarehouseCode);
            Assert.AreEqual(expectedWarehouse.Name, actualWarehouse.Name);
            Assert.AreEqual(expectedWarehouse.Address, actualWarehouse.Address);
        }


        [Test]
        public void TheWarehouseExists()
        {
            var model = new WarehouseCreateViewModel()
            {
                WarehouseCode = "1234567890",
                Name = "WarehouseName",
                Address = "AddressAddress"

            };

            var warehouseService = serviceProvider.GetService<IWarehouseService>();

            Assert.Catch<InvalidOperationException>(() => warehouseService.Create(model), "The warehouse exists.");
        }

        [Test]
        public void TheWarehouseNotExists()
        {
            var model = new WarehouseCreateViewModel()
            {
                WarehouseCode = "1264567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.DoesNotThrowAsync(async () => await service.Create(model));
        }

        [Test]
        public void TheCreateIsSuccessful()
        {
            var model = new WarehouseCreateViewModel()
            {
                WarehouseCode = "1264567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.DoesNotThrowAsync(async () => await service.Create(model));
        }

        [Test]
        public void ThePassedModelIsNull()
        {
            var model = new WarehouseCreateViewModel()
            {
                WarehouseCode = String.Empty,
                Name = String.Empty,
                Address = String.Empty
            };

            var service = serviceProvider.GetService<IWarehouseService>();
            Assert.IsNotNull(service.Create(model));
        }

        [Test]
        public void DeleteWarehouseIfWHIsAsignedToDelivery()
        {
            var service = serviceProvider.GetService<IWarehouseService>();

            service.Delete(new Guid("0259F02A-55EA-450B-9473-761442791864"));
        }

        [Test]
        public void NotDeleteWarehouseIfWHIsNull()
        {
            var service = serviceProvider.GetService<IDriverService>();

            Assert.Catch<Exception>(() => service.Delete(new Guid("0259F05A-55EA-453B-9473-761442791864")));
        }


        [Test]
        public void ShouldThrowExceptionIfEditWarehouseIsNull()
        {
            WarehouseCreateViewModel model = new WarehouseCreateViewModel()
            { };

            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.Catch<Exception>(() => service.Edit(new Guid("0259F02A-55EA-450B-9473-761442791864"), model));
        }

        [Test]
        public void ShouldThrowExceptionIfEditWarehouseCodeExistAndIdIsDifferent()
        {
            WarehouseCreateViewModel model = new WarehouseCreateViewModel()
            {
                WarehouseCode = "1234567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.Catch<Exception>(() => service.Edit(new Guid("0259F02A-55BA-490B-9473-761442011864"), model), "The warehouse code exists.");
        }

        [Test]
        public void ShouldNotThrowExceptionIfEditWarehouseIsSuccessful()
        {
            WarehouseCreateViewModel model = new WarehouseCreateViewModel()
            {
                WarehouseCode = "1534567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.DoesNotThrow(() => service.Edit(new Guid("0259F02A-55EA-450B-9473-761442791864"), model));
        }

        [Test]
        public void ShouldReturnNullIfGetWarehouseViewModelByGuidIsEmpty()
        {
            var service = serviceProvider.GetService<IWarehouseService>();

            Assert.Null(service.GetWarehouseViewModelByGuid(new Guid("a259B72A-55EA-450B-9473-761442791864")));
        }

        [Test]
        public void ShouldReturnCorrectResultForGetWarehouseViewModelByGuid()
        {
            var service = serviceProvider.GetService<IWarehouseService>();

            var res = service.GetWarehouseViewModelByGuid(new Guid("0259F02A-55EA-450B-9473-761442791864"));

            var warehouseList = new List<WarehouseCreateViewModel>();
            var warehouse = new WarehouseCreateViewModel();
            warehouse.WarehouseCode = "1234567890";
            warehouse.Name = "WarehouseName";
            warehouse.Address = "AddressAddress";

            warehouseList.Add(warehouse);

            var expected = (IEnumerable<WarehouseCreateViewModel>)warehouseList;

            var expectedWarehouse = expected.First();
            var actualWarehouse = res;

            Assert.AreEqual(expectedWarehouse.WarehouseCode, actualWarehouse.WarehouseCode);
            Assert.AreEqual(expectedWarehouse.Name, actualWarehouse.Name);
            Assert.AreEqual(expectedWarehouse.Address, actualWarehouse.Address);
        }


        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            var warehouse = new Warehouse()
            {
                Id = new Guid("0259F02A-55EA-450B-9473-761442791864"),
                WarehouseCode = "1234567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            var delivery = new Delivery()
            {
                Id = new Guid("0278F02A-55EA-450B-9473-761442791864"),
                DeliveryRef = "0278F02A-55EA-450B-9473-761442791994",
                PickWarehouseId = new Guid("0259F02A-55EA-450B-9473-761442791864"),
                TruckId = new Guid("0259F02A-55EA-450B-5473-761442791864"),
                UserId = "0278U09A-55EA-450B-9473-761442791994"
            };

            await repo.AddAsync(delivery);
            await repo.AddAsync(warehouse);
            await repo.SaveChangesAsync();
        }
    }
}
