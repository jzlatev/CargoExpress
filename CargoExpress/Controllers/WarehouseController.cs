namespace CargoExpress.Controllers
{
	using CargoExpress.Areas.Admin.Constants;
	using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.Differencing;

    [Authorize(Roles = UserConstants.Roles.Moderator)]
    [Authorize(Roles = UserConstants.Roles.Administrator)]
    public class WarehouseController : BaseController
    {
        private readonly IWarehouseService warehouseService;

        public WarehouseController(IWarehouseService _warehouseService)
        {
            this.warehouseService = _warehouseService;
            this.EntityName = "Warehouse";
        }

        public IActionResult All([FromQuery] WarehouseSearchQueryModel query)
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

            return RedirectToList();
        }

        public IActionResult Edit([FromQuery] string guid)
        {
            WarehouseCreateViewModel? model = null;

            try
            {
                model = warehouseService.GetWarehouseViewModelByGuid(new Guid(guid));
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
        public IActionResult Edit([FromQuery] string guid, WarehouseCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                this.warehouseService.Edit(new Guid(guid), model);
            }
            catch (InvalidOperationException ioe)
            {
                this.ModelState.AddModelError(nameof(model.WarehouseCode), ioe.Message);
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
                warehouseService.Delete(new Guid(guid));
            }
            catch (Exception)
            { }

            return RedirectToList();
        }
    }
}
