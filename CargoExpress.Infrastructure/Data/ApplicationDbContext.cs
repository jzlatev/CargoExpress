using CargoExpress.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoExpress.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DeliveryTruck>()
                .HasKey(dt => new { dt.DeliveryId, dt.TruckId });
        }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Delivery> Deliveries { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Truck> Trucks { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<DeliveryTruck> DeliveriesTrucks { get; set; }
    }
}