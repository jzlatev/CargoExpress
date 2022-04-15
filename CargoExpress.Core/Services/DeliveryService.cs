namespace CargoExpress.Core.Services
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Models.Enums;
    using CargoExpress.Infrastructure.Data.Models;
    using CargoExpress.Infrastructure.Data.Repositories;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DeliveryService : IDeliveryService
    {
        private readonly IApplicationDbRepository repo;

        public DeliveryService(IApplicationDbRepository _repo)
        {
            this.repo = _repo;
        }

        public (IEnumerable<DeliveryAllViewModel>, int totalDeliveries) All(string searchTerm, DeliverySorting deliverySorting, int currentPage, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var deliveryQuery = repo.All<Delivery>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                deliveryQuery = deliveryQuery.Where(d =>
                d.DeliveryRef.ToLower().Contains(searchTerm.ToLower()) ||
                d.DeliveryAddress.ToLower().Contains(searchTerm.ToLower()) ||
                d.PickAddress.ToLower().Contains(searchTerm.ToLower()));
            }

            deliveryQuery = deliverySorting switch
            {
                DeliverySorting.CreatedAt => deliveryQuery.OrderByDescending(c => c.CreatedAt),
                DeliverySorting.PickedAt => deliveryQuery.OrderByDescending(p => p.PickedAt),
                DeliverySorting.DeliveredAt => deliveryQuery.OrderByDescending(d => d.DeliveredAt),
                _ => deliveryQuery.OrderByDescending(d => d.CreatedAt)
            };

            if (!(user.IsInRole("Administrator") || user.IsInRole("Moderator")))
            {
                deliveryQuery = deliveryQuery.Where(c => c.UserId == userId);
            }

            var totalDeliveries = deliveryQuery.Count();

            var deliveries = deliveryQuery
                .Skip((currentPage - 1) * DeliverySearchQueryModel.DeliveriesPerPage)
                .Take(DeliverySearchQueryModel.DeliveriesPerPage)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DeliveryAllViewModel
                {
                    Id = d.Id.ToString(),
                    DeliveryRef = d.DeliveryRef,
                    CreatedAt = d.CreatedAt.ToString(),
                    PickedAt = d.PickedAt.ToString(),
                    DeliveredAt = d.DeliveredAt.ToString(),
                    PickWarehouseId = d.PickWarehouseId.ToString(),
                    PickAddress = d.PickAddress,
                    DeliveryWarehouseId = d.DeliveryWarehouseId.ToString(),
                    DeliveryAddress = d.DeliveryAddress,
                    Status = d.getStatus()
                })
                .ToList();

            return (deliveries, totalDeliveries);
        }

        public Task Create(DeliveryCreateViewModel model, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            Delivery delivery = new Delivery
            {   
                PickWarehouseId = model.PickWarehouseId,
                PickAddress = model.PickAddress,
                DeliveryWarehouseId = model.DeliveryWarehouseId,
                DeliveryAddress = model.DeliveryAddress,
                UserId = userId.ToString(),
                TruckId = model.TruckId,
            };

            var deliveryRef = delivery.DeliveryRef;

            var cargoId = model.CargoId;

            Cargo? cargo = repo.All<Cargo>()
               .Where(d => d.Id == cargoId)
               .ToList()
               .FirstOrDefault();

            cargo.DeliveryId = delivery.Id;


            if (repo.All<Delivery>().Any(d => d.DeliveryRef == deliveryRef) && deliveryRef != null)
            {
                throw new FormException(nameof(model.CargoId), "The delivery exists.");
            }

            if (delivery.PickWarehouseId == delivery.DeliveryWarehouseId)
            {
                throw new FormException(nameof(model.DeliveryWarehouseId), "The loading warehouse coincides with the delivery warehouse.");
            }

            try
            {
                repo.AddAsync(delivery);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            { }

            return Task.CompletedTask;
        }

        public void Delete(Guid guid)
        {
            Delivery? delivery = repo.All<Delivery>()
                .Where(d => d.Id == guid)
                .ToList()
                .FirstOrDefault();

            Cargo? cargo = repo.All<Cargo>()
                .Where(t => t.DeliveryId == delivery.Id)
                .FirstOrDefault();
            if (cargo != null)
            {
                cargo.DeliveryId = null;
            }

            Truck? truck = repo.All<Truck>()
                .Where(t => t.DeliveryId == delivery.Id)
                .FirstOrDefault();

            if (truck != null)
            {
                truck.DeliveryId = null;
            }

            if (delivery == null)
            {
                throw new Exception();
            }

            repo.Delete(delivery);
            repo.SaveChanges();
        }

        public void Edit(Guid guid, DeliveryCreateViewModel model, ClaimsPrincipal user)
        {
            Delivery? delivery = repo.All<Delivery>()
                .Where(d => d.Id == guid)
                .ToList()
                .FirstOrDefault();

            if (delivery == null)
            {
                throw new Exception();
            }

            if (model.PickWarehouseId == model.DeliveryWarehouseId)
            {
                throw new FormException(nameof(model.DeliveryWarehouseId), "The loading warehouse coincides with the delivery warehouse.");
            }

            if (model.DeliveredAt != null && model.PickedAt == null)
            {
                throw new FormException(nameof(model.PickedAt), "Missing loading date.");
            }

            //delivery.cargoId = model.CargoId ?????

            delivery.PickedAt = model.PickedAt;
            delivery.DeliveredAt = model.DeliveredAt;
            delivery.PickWarehouseId = model.PickWarehouseId;
            delivery.PickAddress = model.PickAddress;
            delivery.DeliveryWarehouseId = model.DeliveryWarehouseId;
            delivery.DeliveryAddress = model.DeliveryAddress;
            delivery.TruckId = model.TruckId;

            repo.SaveChanges();
        }

        public DeliveryCreateViewModel? GetDeliveryViewModelByGuid(Guid guid)
        {
            var deliveryList = repo.All<Delivery>()
                .Where(d => d.Id == guid)
                .Select(d => new DeliveryCreateViewModel
                {
                    PickedAt = d.PickedAt,
                    DeliveredAt = d.DeliveredAt,
                    PickWarehouseId = d.PickWarehouseId,
                    PickAddress = d.PickAddress,
                    DeliveryWarehouseId = d.DeliveryWarehouseId,
                    DeliveryAddress = d.DeliveryAddress
                })
                .ToList();

            if (deliveryList.Count == 0)
            {
                return null;
            }

            return deliveryList.First();
        }

        public Delivery? GetDeliveryByGuid(Guid guid)
        {
            var deliveryList = repo.All<Delivery>()
                .Where(d => d.Id == guid)
                .Select(d => d)
                .ToList();

            if (deliveryList.Count == 0)
            {
                return null;
            }

            return deliveryList.First();
        }

        public void PopulateWarehouse(DeliveryCreateViewModel model)
        {
            var warehouses = repo.All<Warehouse>().Select(w => w).ToList();
            Dictionary<string, string> availableWarehouses = new Dictionary<string, string>();

            foreach (var warehouse in warehouses)
            {
                availableWarehouses.Add(warehouse.Id.ToString(), warehouse.Name);
            }

            model.WarehouseNames = availableWarehouses;
        }

        public void PopulateTruck(DeliveryCreateViewModel model)
        {
            var trucks = repo.All<Truck>().Where(t => t.DeliveryId == null && t.DriverId != null).Select(t => t).ToList();

            if (trucks == null)
            {
                throw new FormException(nameof(model.TruckId), "Тhere are no free trucks.");
            }

            Dictionary<string, string> availableTrucks = new Dictionary<string, string>();

            foreach (var truck in trucks)
            {
                availableTrucks.Add(truck.Id.ToString(), truck.PlateNumber);
            }

            model.TruckPlates = availableTrucks;
        }

        public void PopulateCargo(DeliveryCreateViewModel model, ClaimsPrincipal user)
        {
            var cargosQuery = repo.All<Cargo>().Where(c => c.DeliveryId == null).Select(c => c);

            if (!(user.IsInRole("Administrator") || user.IsInRole("Moderator")))
            {
                cargosQuery = cargosQuery.Where(c => c.UserId == user.FindFirstValue(ClaimTypes.NameIdentifier));
            }

            var cargos = cargosQuery.ToList();

            if (cargos == null)
            {
                throw new FormException(nameof(model.CargoId), "Тhere are no available cargos.");
            }

            Dictionary<string, string> availableCargos = new Dictionary<string, string>();

            foreach (var cargo in cargos)
            {
                availableCargos.Add(cargo.Id.ToString(), cargo.Name);
            }

            model.CargoNames = availableCargos;
        }
        public void Pick(Guid guid)
        {
            var delivery = GetDeliveryByGuid(guid);
            delivery.PickedAt = DateTime.Now;

            repo.SaveChanges();
        }

        public void Deliver(Guid guid)
        {
            var deliveryField = repo.All<Delivery>()
                .Where(d => d.Id == guid)
                .Select(d => d.PickedAt)
                .FirstOrDefault();

            if (deliveryField != null)
            {
                var delivery = GetDeliveryByGuid(guid);
                delivery.DeliveredAt = DateTime.Now;

                repo.SaveChanges();
            }
        }
    }  
}
