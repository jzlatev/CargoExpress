namespace CargoExpress.Areas.Admin.Contracts
{
    using CargoExpress.Areas.Admin.Models;
    using CargoExpress.Infrastructure.Data.Identity;

    public interface IUserService
    {
        Task<IEnumerable<UserAllViewModel>> GetUsers();

        Task<UserEditViewModel> GetUserVModelById(string id);

        Task<bool> Edit(UserEditViewModel model);

        Task<ApplicationUser> GetUserById(string id);
    }
}
