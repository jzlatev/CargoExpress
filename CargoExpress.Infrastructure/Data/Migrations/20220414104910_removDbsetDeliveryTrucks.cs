using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class removDbsetDeliveryTrucks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveriesTrucks_Deliveries_DeliveryId",
                table: "DeliveriesTrucks");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveriesTrucks_Trucks_TruckId",
                table: "DeliveriesTrucks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveriesTrucks",
                table: "DeliveriesTrucks");

            migrationBuilder.RenameTable(
                name: "DeliveriesTrucks",
                newName: "DeliveryTruck");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveriesTrucks_TruckId",
                table: "DeliveryTruck",
                newName: "IX_DeliveryTruck_TruckId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryTruck",
                table: "DeliveryTruck",
                columns: new[] { "DeliveryId", "TruckId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTruck_Deliveries_DeliveryId",
                table: "DeliveryTruck",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTruck_Trucks_TruckId",
                table: "DeliveryTruck",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTruck_Deliveries_DeliveryId",
                table: "DeliveryTruck");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTruck_Trucks_TruckId",
                table: "DeliveryTruck");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryTruck",
                table: "DeliveryTruck");

            migrationBuilder.RenameTable(
                name: "DeliveryTruck",
                newName: "DeliveriesTrucks");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryTruck_TruckId",
                table: "DeliveriesTrucks",
                newName: "IX_DeliveriesTrucks_TruckId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveriesTrucks",
                table: "DeliveriesTrucks",
                columns: new[] { "DeliveryId", "TruckId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveriesTrucks_Deliveries_DeliveryId",
                table: "DeliveriesTrucks",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveriesTrucks_Trucks_TruckId",
                table: "DeliveriesTrucks",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
