using CargoExpress.Core.Contracts;
using CargoExpress.Core.Models;
using CargoExpress.Core.Models.Enums;
using CargoExpress.Infrastructure.Data.Models;
using CargoExpress.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CargoExpress.Controllers
{
    public class CargoController : BaseController
    {
        private readonly IApplicationDbRepository repo;
        private readonly ICargoService cargoService;

        public CargoController(IApplicationDbRepository _repo, ICargoService _cargoService)
        {
            this.repo = _repo;
            this.cargoService = _cargoService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CargoCreateViewModel model)
        {
            var isExists = this.repo.All<Cargo>().Any(c => c.CargoRef == model.CargoRef);

            if (isExists)
            {
                this.ModelState.AddModelError(nameof(model.CargoRef), "The cargo has already been added!");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            this.cargoService.Create(model);

            return RedirectToAction(nameof(All));
        }

        public IActionResult All([FromQuery]CargoSearchQueryModel query)
        {
            (query.Cargos, query.TotalCargo) = cargoService.All(query.SearchTerm, query.Sorting, query.CurrentPage);

            return View(query);
        }
    }
}
