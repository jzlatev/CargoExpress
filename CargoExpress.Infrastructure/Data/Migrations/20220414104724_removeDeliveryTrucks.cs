using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class removeDeliveryTrucks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "Cargos");

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryId",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TruckId",
                table: "Deliveries",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_DeliveryId",
                table: "Trucks",
                column: "DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Deliveries_DeliveryId",
                table: "Trucks",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Deliveries_DeliveryId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_DeliveryId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "Deliveries");

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "Cargos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
