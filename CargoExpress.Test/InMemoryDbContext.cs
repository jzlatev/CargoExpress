namespace CargoExpress.Test
{
    using CargoExpress.Infrastructure.Data;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    public class InMemoryDbContext
    {
        private readonly SqliteConnection connection;
        private readonly DbContextOptions<ApplicationDbContext> dbContextOption;

        public InMemoryDbContext()
        {
            connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            dbContextOption = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new ApplicationDbContext(dbContextOption);

            context.Database.EnsureCreated();
        }

        public ApplicationDbContext CreateContext() => new ApplicationDbContext(dbContextOption);

        public void Dispose() => connection.Dispose();
    }
}