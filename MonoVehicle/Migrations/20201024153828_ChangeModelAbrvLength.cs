using Microsoft.EntityFrameworkCore.Migrations;

namespace MonoVehicle.Migrations
{
    public partial class ChangeModelAbrvLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Abrv",
                table: "VehicleModel",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Abrv",
                table: "VehicleModel",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
