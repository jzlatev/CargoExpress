using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class addTypeCargo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDangerous",
                table: "Cargos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDangerous",
                table: "Cargos");
        }
    }
}
