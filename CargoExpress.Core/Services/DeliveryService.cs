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
                d.PickAddress.ToLower().Contains(searchTerm.ToLower()) ||
                d.Warehouse.Name.ToLower().Contains(searchTerm.ToLower()));
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
                    Status = d.getStatus(),
                    TruckId = d.TruckId,
                    Cargos = d.Cargos.Where(c => c.DeliveryId == d.Id).ToList(),
                    PickWarehouseName = repo.All<Warehouse>().Where(w => w.Id == d.PickWarehouseId).AsQueryable().Select(w => w.Name).First(),
                    DeliveryWarehouseName = repo.All<Warehouse>().Where(w => w.Id == d.DeliveryWarehouseId).AsQueryable().Select(w => w.Name).First(),

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

            if (delivery.PickWarehouseId == delivery.DeliveryWarehouseId && 
                !(delivery.PickWarehouseId == null || delivery.DeliveryWarehouseId == null))
            {
                throw new FormException(nameof(model.DeliveryWarehouseId), "The loading warehouse coincides with the delivery warehouse.");
            }

            if (delivery.PickAddress == delivery.DeliveryAddress && 
                !(delivery.PickAddress == null || delivery.PickAddress.ToString().Trim() == "" 
                || delivery.DeliveryAddress == null || delivery.DeliveryAddress.ToString().Trim() == ""))
            {
                throw new FormException(nameof(model.DeliveryAddress), "The loading address coincides with the delivery address.");
            }

            if (delivery.PickWarehouseId == null && 
                (delivery.PickAddress == null || delivery.PickAddress.ToString().Trim() == ""))
            {
                throw new FormException(nameof(model.PickAddress), "You must specify warehouse or address for picking");
            }

            if (delivery.PickWarehouseId != null && 
                (delivery.PickAddress != null && delivery.PickAddress.ToString().Trim() != ""))
            {
                throw new FormException(nameof(model.PickAddress), "You must specify only warehouse or address for picking");
            }

            if (delivery.DeliveryWarehouseId == null && 
                (delivery.DeliveryAddress == null || delivery.DeliveryAddress.ToString().Trim() == ""))
            {
                throw new FormException(nameof(model.DeliveryAddress), "You must specify warehouse or address for delivery");
            }

            if (delivery.DeliveryWarehouseId != null && 
                (delivery.DeliveryAddress != null && delivery.DeliveryAddress.ToString().Trim() != ""))
            {
                throw new FormException(nameof(model.DeliveryAddress), "You must specify only warehouse or address for delivery");
            }

            try
            {
                repo.AddAsync(delivery);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Error occured");
            }

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

            if (model.PickWarehouseId == model.DeliveryWarehouseId && 
                !(model.PickWarehouseId == null || model.DeliveryWarehouseId == null))
            {
                throw new FormException(nameof(model.DeliveryWarehouseId), "The loading warehouse coincides with the delivery warehouse.");
            }

            if (model.PickAddress == model.DeliveryAddress && 
                !(model.PickAddress == null || model.PickAddress.ToString().Trim() == "" 
                || model.DeliveryAddress == null || model.DeliveryAddress.ToString().Trim() == ""))
            {
                throw new FormException(nameof(model.DeliveryAddress), "The loading address coincides with the delivery address.");
            }

            if (model.PickWarehouseId == null && 
                (model.PickAddress == null || model.PickAddress.ToString().Trim() == ""))
            {
                throw new FormException(nameof(model.PickAddress), "You must specify warehouse or address for picking");
            }

            if (model.PickWarehouseId != null && 
                (model.PickAddress != null && model.PickAddress.ToString().Trim() != ""))
            {
                throw new FormException(nameof(model.PickAddress), "You must specify only warehouse or address for picking");
            }

            if (model.DeliveryWarehouseId == null && 
                (model.DeliveryAddress == null || model.DeliveryAddress.ToString().Trim() == ""))
            {
                throw new FormException(nameof(model.DeliveryAddress), "You must specify warehouse or address for delivery");
            }

            if (model.DeliveryWarehouseId != null && 
                (model.DeliveryAddress != null && model.DeliveryAddress.ToString().Trim() != ""))
            {
                throw new FormException(nameof(model.DeliveryAddress), "You must specify only warehouse or address for delivery");
            }

            if (model.TruckId == null && 
                (model.PickedAt != null || model.DeliveredAt != null || (model.PickedAt != null && model.DeliveredAt != null)))
            {
                throw new FormException(nameof(model.TruckId), "You must specify truck for delivery");
            }

            if (user.IsInRole("Administrator") || user.IsInRole("Moderator"))
            { 
                delivery.PickedAt = model.PickedAt;
                delivery.DeliveredAt = model.DeliveredAt;
            }

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
                    DeliveryAddress = d.DeliveryAddress,
                    TruckId = d.TruckId,
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

       public void Pick(Guid guid)
        {
            var delivery = GetDeliveryByGuid(guid);

            if (delivery.TruckId == null)
            {
                return;
            }

            if (delivery != null && delivery.PickedAt != null)
            {
                return;
            }

            delivery.PickedAt = DateTime.Now;

            repo.SaveChanges();
        }

        public void Deliver(Guid guid)
        {
            var delivery = GetDeliveryByGuid(guid);

            if (delivery.TruckId == null)
            {
                return;
            }

            if (delivery != null && delivery.DeliveredAt != null)
            {
                return;
            }

            delivery.DeliveredAt = DateTime.Now;

            repo.SaveChanges();
        }
    }  
}
