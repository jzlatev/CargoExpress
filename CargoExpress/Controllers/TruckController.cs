namespace CargoExpress.Controllers
{
	using CargoExpress.Areas.Admin.Constants;
	using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = UserConstants.Roles.Moderator+","+UserConstants.Roles.Administrator)]
//    [Authorize(Roles = UserConstants.Roles.Administrator)]
    public class TruckController : BaseController
    {
        private readonly ITruckService truckService;

        public TruckController(ITruckService _truckService)
        {
            this.truckService = _truckService;
            this.EntityName = "Truck";
        }

        public IActionResult All([FromQuery]TruckSearchQueryModel query)
        {
            (query.Trucks, query.TotalTrucks) = truckService.All(query.SearchTerm, query.CurrentPage);

            return View(query);
        }

        public IActionResult Create()
        {
            var model = new TruckCreateViewModel();
            truckService.PopulateAvailableDrivers(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(TruckCreateViewModel model)
        {
            truckService.PopulateAvailableDrivers(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.truckService.Create(model);
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

        public IActionResult Edit([FromQuery] string guid)
        {
            TruckCreateViewModel? model = null;

            try
            {
                model = truckService.GetTruckViewModelByGuid(new Guid(guid));
            }
            catch (FormException e)
            {
                this.ModelState.AddModelError(e.InputName, e.ErrorMessage);

            }
            catch (Exception)
            {
                return RedirectToList();
            }


            if (model == null)
            {
                return RedirectToList();
            }

            truckService.PopulateAvailableDrivers(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit([FromQuery] string guid, TruckCreateViewModel model)
        {
            
            truckService.PopulateAvailableDrivers(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.truckService.Edit(new Guid(guid), model);
            }
            catch (FormException fe)
            {
                this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);

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
                truckService.Delete(new Guid(guid));
            }
            catch (Exception)
            {
            }

            return RedirectToList();
        }
    }
}
