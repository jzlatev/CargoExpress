using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class addCargoDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cargos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cargos");
        }
    }
}
