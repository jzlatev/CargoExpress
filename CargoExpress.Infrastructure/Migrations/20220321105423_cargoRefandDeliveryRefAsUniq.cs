using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class cargoRefandDeliveryRefAsUniq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryRef",
                table: "Deliveries",
                column: "DeliveryRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_CargoRef",
                table: "Cargos",
                column: "CargoRef",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryRef",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_CargoRef",
                table: "Cargos");
        }
    }
}
