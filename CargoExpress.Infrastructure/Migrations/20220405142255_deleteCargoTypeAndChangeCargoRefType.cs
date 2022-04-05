using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class deleteCargoTypeAndChangeCargoRefType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDangerous",
                table: "Cargos");

            migrationBuilder.AlterColumn<Guid>(
                name: "CargoRef",
                table: "Cargos",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CargoRef",
                table: "Cargos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 36);

            migrationBuilder.AddColumn<bool>(
                name: "IsDangerous",
                table: "Cargos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
