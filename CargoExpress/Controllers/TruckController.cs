namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public class TruckController : BaseController
    {
        private readonly ITruckService truckService;

        public TruckController(ITruckService _truckService)
        {
            this.truckService = _truckService;
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
                this.ModelState.AddModelError(e.InputName,e.ErrorMessage);

            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(All));
        }
    }
}
