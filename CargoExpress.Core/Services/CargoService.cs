using CargoExpress.Core.Contracts;
using CargoExpress.Core.Exceptions;
using CargoExpress.Core.Models;
using CargoExpress.Core.Models.Enums;
using CargoExpress.Infrastructure.Data.Identity;
using CargoExpress.Infrastructure.Data.Models;
using CargoExpress.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CargoExpress.Core.Services
{
    public class CargoService : ICargoService
    {
        private readonly IApplicationDbRepository repo;
        private readonly UserManager<ApplicationUser> userManager;

        public CargoService(IApplicationDbRepository _repo, UserManager<ApplicationUser> _userManager)
        {
            this.repo = _repo;
            userManager = _userManager;
        }

        public (IEnumerable<CargoAllViewModel>, int totalCargo) All(string? searchTerm, CargoSorting sorting, int currentPage, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var cargoQuery = repo.All<Cargo>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cargoQuery = cargoQuery.Where(c =>
                c.Name.ToLower().Contains(searchTerm.ToLower()) ||
                c.Description.ToLower().Contains(searchTerm.ToLower()) ||
                c.CargoRef.ToString().ToLower().Contains(searchTerm.ToLower()));
            }

            cargoQuery = sorting switch
            {
                CargoSorting.Date => cargoQuery.OrderByDescending(d => d.CreatedAt),
                CargoSorting.Name => cargoQuery.OrderByDescending(n => n.Name),
                CargoSorting.Weight => cargoQuery.OrderByDescending(w => w.Weight),
                CargoSorting.Type => cargoQuery.OrderByDescending(t => t.IsDangerous),
                _ => cargoQuery.OrderByDescending(r => r.CreatedAt)
            };

            if (!(user.IsInRole("Administrator") || user.IsInRole("Moderator")))
            {
                cargoQuery = cargoQuery.Where(c => c.UserId == userId);
            }

            var totalNumCargo = cargoQuery.Count();

            var userData = userManager.Users;

            var userNames = userData.Select(u => new CargoAllUserNames
            {
                Guid = u.Id.ToString(),
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            })
            .ToDictionary(keySelector: u => u.Guid);

            var allCargosCurrentPage = cargoQuery
                .Skip((currentPage - 1) * CargoSearchQueryModel.CargosPerPage) // Curnt page - 1 * number of all pages
                .Take(CargoSearchQueryModel.CargosPerPage)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CargoAllViewModel
                {
                    Id = c.Id.ToString(),
                    CargoRef = c.CargoRef,
                    Name = c.Name,
                    Weight = c.Weight,
                    CreatedAt = c.CreatedAt,
                    IsDangerous = c.IsDangerous == true ? "Yes" : "No",
                    Description = c.Description,
                    Status = c.Delivery == null ? "Not assigned to delivery" : c.Delivery.getStatus(),
                    Username = userNames[c.UserId].getFullname()
                })
                .ToList();

            return (allCargosCurrentPage, totalNumCargo);
        }

        public Task Create(CargoCreateViewModel model, ClaimsPrincipal user)
        { 
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            Cargo cargo = new Cargo
            {
                Name = model.Name,
                Weight = model.Weight,
                Description = model.Description,
                IsDangerous = model.IsDangerous,
                UserId = userId.ToString(),
                DeliveryId = model.DeliveryId != null ? new Guid(model.DeliveryId) : null
            };

            var cargoRef = cargo.CargoRef;

            if (repo.All<Cargo>().Any(c => c.CargoRef == cargoRef) && cargoRef != null)
            {
                throw new FormException(nameof(cargo.CargoRef), "The delivery exists.");
            }

            try
            {
                repo.AddAsync(cargo);
                repo.SaveChanges();
            }
            catch (InvalidOperationException)
            { }

            return Task.CompletedTask;
        }

        public void Delete(Guid guid)
        {
            Cargo? cargo = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .ToList()
                .FirstOrDefault();

            if (cargo != null)
            {
                repo.Delete(cargo);
                repo.SaveChanges();
            }
        }

        public void Edit(Guid guid, CargoCreateViewModel model)
        {
            Cargo? cargo = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .ToList()
                .FirstOrDefault();

            if (cargo == null)
            {
                throw new Exception();
            }

            cargo.Name = model.Name;
            cargo.Weight = model.Weight;
            cargo.Description = model.Description;
            cargo.IsDangerous = model.IsDangerous;
            cargo.DeliveryId = model.DeliveryId != null ? new Guid(model.DeliveryId) : null;

            repo.SaveChanges();
        }

        public CargoCreateViewModel? GetCargoViewModelByGuid(Guid guid)
        {
            var cargoList = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .Select(c => new CargoCreateViewModel
                {
                    Name = c.Name,
                    Weight = c.Weight,
                    Description = c.Description,
                    IsDangerous = c.IsDangerous,
                    UserId = c.UserId,
                    Status = c.getStatus(),
                    DeliveryId = c.DeliveryId.ToString()
                })
                .ToList();

            if (cargoList.Count == 0)
            {
                return null;
            }

            var cargoCreateViewModel = cargoList.First();

            var cargo = repo.All<Cargo>()
                .Where(c => c.Id == guid)
                .Select(c => c)
                .ToList().First();

            if (cargo.DeliveryId != null)
            {
                var delivery = repo.All<Delivery>()
                    .Where(d => d.Id == cargo.DeliveryId)
                    .Select(d => d)
                    .ToList().First();

                cargo.Delivery = delivery;
            }

            cargoCreateViewModel.Status = cargo.getStatus();

            return cargoCreateViewModel;
        }

        public void PopulateAvailableDeliveries(CargoCreateViewModel model, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var deliveries = repo.All<Delivery>().Select(d => d).Where(d => d.PickedAt == null && d.UserId == userId).ToList();
            Dictionary<string, string> availableDeliveries = new Dictionary<string, string>();

            foreach (var delivery in deliveries)
            {
                availableDeliveries.Add(delivery.Id.ToString(), delivery.DeliveryRef.ToString());
            }

            model.AvailableDeliveries = availableDeliveries;
        }
    }
}
