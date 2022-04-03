namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

        [HttpPost]
        public IActionResult Create(TruckCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.truckService.Create(model);
            }
            catch (InvalidOperationException ioe)
            {
                this.ModelState.AddModelError(nameof(model.PlateNumber), ioe.Message);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("All", "Cargo");
        }
    }
}
