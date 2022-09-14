using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumn_Palletized_To_LicensePlate_And_MixedPallet_To_Pallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MixedPallet",
                table: "Pallets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Palletized",
                table: "LicensePlates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MixedPallet",
                table: "Pallets");

            migrationBuilder.DropColumn(
                name: "Palletized",
                table: "LicensePlates");
        }
    }
}
