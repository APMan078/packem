using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddingNewAddressFieldsToFacilitiesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CustomerFacilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "CustomerFacilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CustomerFacilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "CustomerFacilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StateProvince",
                table: "CustomerFacilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipPostalCode",
                table: "CustomerFacilities",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "City",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "StateProvince",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "ZipPostalCode",
                table: "CustomerFacilities");
        }
    }
}
