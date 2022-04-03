namespace CargoExpress.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Contracts;

    public class DriverController : BaseController
    {
        private readonly IDriverService driverService;

        public DriverController(IDriverService _driverService)
        {
            this.driverService = _driverService;
        }

        public ActionResult All([FromQuery] DriverSearchQueryModel query)
        {
            (query.Drivers, query.TotalDrivers) = driverService.All(query.SearchTerm, query.CurrentPage);

            return View(query);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DriverCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.driverService.Create(model);
            }
            catch (InvalidOperationException ioe)
            {
                this.ModelState.AddModelError(nameof(model.EGN), ioe.Message);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction("All", "Cargo");
        }
    }
}
