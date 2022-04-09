namespace CargoExpress.Areas.Admin.Services
{
    using CargoExpress.Areas.Admin.Contracts;
    using CargoExpress.Areas.Admin.Models;
    using CargoExpress.Infrastructure.Data.Identity;
    using CargoExpress.Infrastructure.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly IApplicationDbRepository repo;

        public UserService(IApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task<bool> Edit(UserEditViewModel model)
        {
            bool result = false;
            var user = await repo.GetByIdAsync<ApplicationUser>(model.Id);
        
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                await repo.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<UserEditViewModel> GetUserById(string id)
        {
            var user = await repo.GetByIdAsync<ApplicationUser>(id);

            if (user == null)
            {
                throw new NullReferenceException("A problem occured.");
            }

            return new UserEditViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<IEnumerable<UserAllViewModel>> GetUsers()
        {
            return await repo.All<ApplicationUser>()
                .Select(u => new UserAllViewModel
                { 
                    Id = u.Id,
                    Email = u.Email,
                    Name = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync();
        }
    }
}
