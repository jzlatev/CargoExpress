namespace CargoExpress.Test
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Models.Enums;
    using CargoExpress.Core.Services;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DeliveryServiceTests
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
                .AddSingleton<IDeliveryService, DeliveryService>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            await SeedDbAsync(repo);
        }

        [Test]
        public void DeleteDeliverySuccessful()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.DoesNotThrow(() => service.Delete(new Guid("0259F02A-15EA-450B-9473-761442791864")));
        }

        [Test]
        public void NotDeleteDriverIfDeliveryIsNull()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Catch<Exception>(() => service.Delete(new Guid("0259F02A-55EA-450B-9473-761442791464")));
        }

        [Test]
        public void ShouldReturnNullIfGetDeliveryViewModelByGuidIsEmpty()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Null(service.GetDeliveryViewModelByGuid(new Guid("a259112A-55EA-450B-9473-761433791864")));
        }

        [Test]
        public void ShouldReturnCorrectResultForGetDeliveryViewModelByGuidIsSUccessful()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            var res = service.GetDeliveryViewModelByGuid(new Guid("0259F02A-15EA-450B-9473-761442791864"));

            var deliveryList = new List<DeliveryCreateViewModel>();
            var delivery = new DeliveryCreateViewModel();
            delivery.PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864");
            delivery.TruckId = new Guid("1159F02A-55EA-450B-9473-761442791864");

            deliveryList.Add(delivery);

            var expected = (IEnumerable<DeliveryCreateViewModel>)deliveryList;

            var expectedDelivery = expected.First();
            var actualDelivery = res;

            Assert.AreEqual(expectedDelivery.PickWarehouseId, actualDelivery.PickWarehouseId);
            Assert.AreEqual(expectedDelivery.TruckId, actualDelivery.TruckId);
        }

        [Test]
        public void ShouldRetunNullIfGetDeliveryByGuidPassedNull()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Null(service.GetDeliveryByGuid(new Guid("a259112A-55EA-450B-9473-761433551864")));
        }

        [Test]
        public void ShouldReturnCorrectResultForGetDeliveryByGuidIsSUccessful()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            var res = service.GetDeliveryByGuid(new Guid("0259F02A-15EA-450B-9473-761442791864"));

            var deliveryList = new List<DeliveryCreateViewModel>();
            var delivery = new DeliveryCreateViewModel();
            delivery.PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864");
            delivery.TruckId = new Guid("1159F02A-55EA-450B-9473-761442791864");

            deliveryList.Add(delivery);

            var expected = (IEnumerable<DeliveryCreateViewModel>)deliveryList;

            var expectedDelivery = expected.First();
            var actualDelivery = res;

            Assert.AreEqual(expectedDelivery.PickWarehouseId, actualDelivery.PickWarehouseId);
            Assert.AreEqual(expectedDelivery.TruckId, actualDelivery.TruckId);
        }

        [Test]
        public void ShouldTNothrowIfTruckIdInPickIsNull()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            var delivery = new Delivery()
            {
                Id = new Guid("0259F02A-15EA-450B-9473-761442791864"),
                DeliveryRef = "0259F02A-15EA-450B-9473-761448591824",
                PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                UserId = "0259F02A-55EA-450B-9473-761122791864"
            };

            Assert.DoesNotThrow(() => service.Pick(new Guid("0259F02A-15EA-450B-9473-761442791864")));
        }

        [Test]
        public void ShouldTNothrowIfPickAtInPickIsNotNull()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            var delivery = new Delivery()
            {
                Id = new Guid("0259F02A-15EA-450B-9473-761442791864"),
                DeliveryRef = "0259F02A-15EA-450B-9473-761448591824",
                PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                CreatedAt = DateTime.Now,
                UserId = "0259F02A-55EA-450B-9473-761122791864"
            };

            Assert.DoesNotThrow(() => service.Pick(new Guid("0259F02A-15EA-450B-9473-761442791864")));
        }

        [Test]
        public void ShouldTNothrowIfTruckIdInDeliverIsNull()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            var delivery = new Delivery()
            {
                Id = new Guid("0259F02A-15EA-450B-9473-761442791864"),
                DeliveryRef = "0259F02A-15EA-450B-9473-761448591824",
                PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                UserId = "0259F02A-55EA-450B-9473-761122791864"
            };

            Assert.DoesNotThrow(() => service.Deliver(new Guid("0259F02A-15EA-450B-9473-761442791864")));
        }

        [Test]
        public void ShouldTNothrowIfDeliveredAtInDeliverIsNotNull()
        {
            var service = serviceProvider.GetService<IDeliveryService>();

            var delivery = new Delivery()
            {
                Id = new Guid("0259F02A-15EA-450B-9473-761442791864"),
                DeliveryRef = "0259F02A-15EA-450B-9473-761448591824",
                PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                DeliveredAt = DateTime.Now,
                UserId = "0259F02A-55EA-450B-9473-761122791864"
            };

            Assert.DoesNotThrow(() => service.Deliver(new Guid("0259F02A-15EA-450B-9473-761442791864")));
        }


        [Test]
        public void ThrowWhenPassEmptyObjecttoCreateMethod()
        {
            var user = new ClaimsPrincipal();

            var model = new DeliveryCreateViewModel()
            {
                
            };

            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Catch<NullReferenceException>(() => service.Create(model, user), "Error occured");
        }

        [Test]
        public void ThrowIfLoadingAndDeliveringWHAreTheSameInCreateMethod()
        {
            var user = new ClaimsPrincipal();

            var model = new DeliveryCreateViewModel()
            {
                PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                DeliveryWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
            };

            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Catch<NullReferenceException>(() => service.Create(model, user), "The loading warehouse coincides with the delivery warehouse.");
        }

        [Test]
        public void ThrowIfLoadingAndDeliveringWHAreBothEmptyInCreateMethod()
        {
            var user = new ClaimsPrincipal();

            var model = new DeliveryCreateViewModel()
            {
                PickWarehouseId = new Guid(),
                DeliveryWarehouseId = new Guid(),
            };

            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Catch<NullReferenceException>(() => service.Create(model, user), "The loading warehouse coincides with the delivery warehouse.");
        }

        [Test]
        public void ThrowIfPickAddrAndDeliveryAddrAreTheSameInCreateMethod()
        {
            var user = new ClaimsPrincipal();

            var model = new DeliveryCreateViewModel()
            {
                PickAddress = "aaa",
                DeliveryAddress = "aaa",
            };

            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Catch<NullReferenceException>(() => service.Create(model, user), "The loading address coincides with the delivery address.");
        }

        [Test]
        public void ThrowIfLoadingAndDeliveringAddrAreBothEmptyInCreateMethod()
        {
            var user = new ClaimsPrincipal();

            var model = new DeliveryCreateViewModel()
            {
                PickAddress = String.Empty,
                DeliveryAddress = String.Empty,
            };

            var service = serviceProvider.GetService<IDeliveryService>();

            Assert.Catch<NullReferenceException>(() => service.Create(model, user), "The loading warehouse coincides with the delivery warehouse.");
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
                Id = new Guid("1159F02A-55EA-450B-9473-761442791864"),
                PlateNumber = "12345678",
                DriverId = new Guid("0259F02A-55EA-450B-9473-761442791864"),
                IsBusy = false,
            };

            var cargo = new Cargo()
            {
                Id = new Guid("0259F11C-15EA-450B-9473-761551791864"),
                Name = "CargoName",
                Weight = 1000,
                Description = "Description for cargo",
                IsDangerous = false,
                UserId = "0259F03D-95EB-420B-9473-761442791864",
                CreatedAt = DateTime.Now,
            };

            var warehouse = new Warehouse()
            {
                Id = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                WarehouseCode = "1234567890",
                Name = "WarehouseName",
                Address = "AddressAddress"
            };

            var delivery = new Delivery()
            {
                Id = new Guid("0259F02A-15EA-450B-9473-761442791864"),
                DeliveryRef = "0259F02A-15EA-450B-9473-761448591824",
                PickWarehouseId = new Guid("5459F02A-55AA-450A-9473-761442791864"),
                TruckId = new Guid("1159F02A-55EA-450B-9473-761442791864"),
                UserId = "0259F02A-55EA-450B-9473-761122791864"
            };

            await repo.AddAsync(driver);
            await repo.AddAsync(truck);
            await repo.AddAsync(cargo);
            await repo.AddAsync(warehouse);
            await repo.AddAsync(delivery);
            await repo.SaveChangesAsync();
        }
    }
}
