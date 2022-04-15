namespace CargoExpress.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using CargoExpress.Core.Models;
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
	using Microsoft.AspNetCore.Authorization;
	using CargoExpress.Areas.Admin.Constants;

    [Authorize(Roles = UserConstants.Roles.Moderator)]
    [Authorize(Roles = UserConstants.Roles.Administrator)]
    public class DriverController : BaseController
    {
        private readonly IDriverService driverService;

        public DriverController(IDriverService _driverService)
        {
            this.driverService = _driverService;
            this.EntityName = "Driver";
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
            catch (FormException fe)
            {
                this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return RedirectToAction(nameof(All));
        }

        public IActionResult Edit([FromQuery] string guid)
        {
            DriverCreateViewModel? model = null;

            try
            {
                model = driverService.GetDriverViewModelByGuid(new Guid(guid));
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
        public IActionResult Edit([FromQuery] string guid, DriverCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                driverService.Edit(new Guid(guid), model);
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
                driverService.Delete(new Guid(guid));
            }
            catch (Exception)
            { }
            
            return RedirectToList();
        }
    }
}
