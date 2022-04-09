namespace CargoExpress.Areas.Admin.Controllers
{
    using CargoExpress.Areas.Admin.Constants;
    using CargoExpress.Areas.Admin.Contracts;
    using CargoExpress.Areas.Admin.Models;
    using CargoExpress.Core.Constants;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Infrastructure.Data.Identity;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public UserController(
            RoleManager<IdentityRole> _roleManager,
            UserManager<ApplicationUser> _userManager,
            IUserService _userService)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            userService = _userService;
            this.EntityName = "User";
        }

        public async Task<IActionResult> All()
        {
            var users = await userService.GetUsers();

            return View(users);
        }

        public async Task<IActionResult> Roles(string id)
        {
            return Ok(id);
        }

        public async Task<IActionResult> Edit(string id)
        {
            UserEditViewModel? model = null;

            try
            {
                model = await userService.GetUserById(id);
            }
            catch (FormException fe)
            {
                this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);
            }
            catch (Exception)
            {
                return RedirectToList();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await userService.Edit(model);
                //ViewData[ResponseConstant.SuccessMessage] = "The information was updated.";
            }
            catch (FormException fe)
            {
                this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);
                //ViewData[ResponseConstant.ErrorMessage] = "The information was NOT updated.";
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

        public async Task<IActionResult> CreateRole()
        {
            //await roleManager.CreateAsync(new IdentityRole()
            //{
            //    Name = UserConstants.Roles.Administrator
            //});

            return Ok();
        }
    }
}
