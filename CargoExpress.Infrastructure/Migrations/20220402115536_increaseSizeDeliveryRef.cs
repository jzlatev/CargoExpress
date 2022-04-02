using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoExpress.Infrastructure.Data.Migrations
{
    public partial class increaseSizeDeliveryRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeliveryRef",
                table: "Deliveries",
                type: "nvarchar(38)",
                maxLength: 38,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeliveryRef",
                table: "Deliveries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(38)",
                oldMaxLength: 38);
        }
    }
}
