using CargoExpress.Core.Contracts;
using CargoExpress.Core.Exceptions;
using CargoExpress.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var model = new CargoCreateViewModel();
            cargoService.PopulateAvailableDeliveries(model, User);
            return View(model);
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
                this.cargoService.Create(model, User);
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
            (query.Cargos, query.TotalCargo) = cargoService.All(query.SearchTerm, query.Sorting, query.CurrentPage, User);

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

            if (model.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !(User.IsInRole("Administrator") || User.IsInRole("Moderator")))
            {
                return new ForbidResult();
            }

            if (model.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (model.Status == "In progress" || model.Status == "Done"))
            {
                return new ForbidResult();
            }

            if (model == null)
            {
                return RedirectToList();
            }

            cargoService.PopulateAvailableDeliveries(model, User);

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromQuery] string guid, CargoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cargoViewModel = cargoService.GetCargoViewModelByGuid(new Guid(guid));

            model.UserId = cargoViewModel.UserId;

            if (model.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !(User.IsInRole("Administrator") || User.IsInRole("Moderator")))
            {
                return new ForbidResult();
            }

            if (!(User.IsInRole("Administrator") || User.IsInRole("Moderator")) && model.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (model.Status == "In progress" || model.Status == "Done"))
            {
                return new ForbidResult();
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
            CargoCreateViewModel? model = null;

            model = cargoService.GetCargoViewModelByGuid(new Guid(guid));
            

            if (model.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !(User.IsInRole("Administrator") || User.IsInRole("Moderator")))
            {
                return new ForbidResult();
            }

            if (model.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && (model.Status == "In progress" || model.Status == "Done"))
            {
                return new ForbidResult();
            }

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
