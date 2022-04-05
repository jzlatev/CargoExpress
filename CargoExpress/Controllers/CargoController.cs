using CargoExpress.Core.Contracts;
using CargoExpress.Core.Exceptions;
using CargoExpress.Core.Models;
using CargoExpress.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CargoExpress.Controllers
{
    public class CargoController : BaseController
    {
        private readonly ICargoService cargoService;

        public CargoController(ICargoService _cargoService)
        {
            this.cargoService = _cargoService;
            this.EntityName = "Cargo";
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CargoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.cargoService.Create(model);
            }
            catch (FormException e)
            {
                this.ModelState.AddModelError(e.InputName, e.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToList();
        }

        public IActionResult All([FromQuery]CargoSearchQueryModel query)
        {
            (query.Cargos, query.TotalCargo) = cargoService.All(query.SearchTerm, query.Sorting, query.CurrentPage);

            return View(query);
        }

        public IActionResult Edit([FromQuery] string guid)
        {
            CargoCreateViewModel? model = null;

            try
            {
                model = cargoService.GetCargoViewModelByGuid(new Guid(guid));
            }
            catch (FormException fe)
            {
                this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);
            }
            catch (Exception)
            {
                return RedirectToList();
            }

            if (model == null)
            {
                return RedirectToList();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromQuery] string guid, CargoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.cargoService.Edit(new Guid(guid), model);
            }
            catch (FormException e)
            {
                this.ModelState.AddModelError(e.InputName, e.ErrorMessage);
            }
            catch (Exception)
            {
                return RedirectToList();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToList();
        }

        [HttpPost]
        public IActionResult Delete([FromQuery] string guid)
        {
            try
            {
                cargoService.Delete(new Guid(guid));
            }
            catch (Exception)
            { }

            return RedirectToList();
        }
    }
}
