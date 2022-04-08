namespace CargoExpress.Infrastructure.Data.Identity
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class ApplicationUser : IdentityUser
    {
        [MaxLength(35)]
        public string? FirstName { get; set; }

        [MaxLength(35)]
        public string? LastName { get; set; }
    }
}
