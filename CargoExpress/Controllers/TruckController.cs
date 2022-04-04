namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;

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
            catch (FormException e)
            {
                this.ModelState.AddModelError(e.InputName, e.ErrorMessage);

            }
            catch (Exception e)
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
            //TruckCreateViewModel? model = null;

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
