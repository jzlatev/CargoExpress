namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.VisualBasic.Syntax;

    public class UserController : Controller
    {
        public UserController(UserService userService)
        {
            UserService = userService;
        }

        public UserService UserService { get; }

        [Authorize(Roles = "Administrator")]
        public IActionResult IsAdmin()
        {
            return View();
        }

        public IActionResult MakeAdmin()
        {
            var user = User;
            UserService.makeAdmin(user);
            return View();
        }


    }
}
