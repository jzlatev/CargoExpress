namespace CargoExpress.Areas.Admin.Controllers
{
    using CargoExpress.Areas.Admin.Constants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = UserConstants.Roles.Administrator)]
    [Area("Admin")]
    public abstract class BaseController : Controller
    {
        protected string? EntityName = null;

        protected IActionResult RedirectToList()
        {
            return RedirectToAction("All", EntityName);
        }
    }
}
