using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoExpress.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
    }
}
