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

        public (IEnumerable<DeliveryAllViewModel>, int totalDeliveries) All(string searchTerm, DeliverySorting deliverySorting, int currentPage)
        {
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
                })
                .ToList();

            return (deliveries, totalDeliveries);
        }

        public Task Create(DeliveryCreateViewModel model, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            Delivery delivery = new Delivery
            {
                PickedAt = model.PickedAt,
                DeliveredAt = model.DeliveredAt,
                PickWarehouseId = model.PickWarehouseId,
                PickAddress = model.PickAddress,
                DeliveryWarehouseId = model.DeliveryWarehouseId,
                DeliveryAddress = model.DeliveryAddress,
                UserId = userId.ToString()
            };

            var deliveryRef = delivery.DeliveryRef;

            if (repo.All<Delivery>().Any(d => d.DeliveryRef == deliveryRef) && deliveryRef != null)
            {
                throw new FormException(nameof(model.PickedAt), "The delivery exists.");
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

            delivery.PickedAt = model.PickedAt;
            delivery.DeliveredAt = model.DeliveredAt;
            delivery.PickWarehouseId = model.PickWarehouseId;
            delivery.PickAddress = model.PickAddress;
            delivery.DeliveryWarehouseId = model.DeliveryWarehouseId;
            delivery.DeliveryAddress = model.DeliveryAddress;

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
    }
}
