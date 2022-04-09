namespace CargoExpress.Areas.Admin.Contracts
{
    using CargoExpress.Areas.Admin.Models;

    public interface IUserService
    {
        Task<IEnumerable<UserAllViewModel>> GetUsers();

        Task<UserEditViewModel> GetUserById(string id);

        Task<bool> Edit(UserEditViewModel model);
    }
}
