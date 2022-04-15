using CargoExpress.Infrastructure.Data.Identity;
using CargoExpress.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoExpress.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Cargo>()
                .HasIndex(c => c.CargoRef)
                .IsUnique();

            builder.Entity<Delivery>()
                .HasIndex(d => d.DeliveryRef)
                .IsUnique();

            builder.Entity<Warehouse>()
                .HasIndex(w => w.WarehouseCode)
                .IsUnique();
        }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Delivery> Deliveries { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Truck> Trucks { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }
    }
}