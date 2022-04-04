using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoExpress.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected string? EntityName = null;

        protected IActionResult RedirectToList()
        {
            return RedirectToAction("All", EntityName);
        }
    }

}
