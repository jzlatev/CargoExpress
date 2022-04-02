namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;

    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService warehouseService;

        public WarehouseController(IWarehouseService _warehouseService)
        {
            this.warehouseService = _warehouseService;
        }

        public IActionResult All([FromQuery]WarehouseSearchQueryModel query)
        {
            (query.Warehouses, query.TotalWarehouses) = warehouseService.All(query.SearchTerm, query.CurrentPage);

            return View(query);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(WarehouseCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.warehouseService.Create(model);
            }
            catch (InvalidOperationException ioe)
            {
                this.ModelState.AddModelError(nameof(model.WarehouseCode), ioe.Message);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(All));
        }
    }
}
