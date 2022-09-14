using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class RemoveColumnsAtSaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerNo",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToAddress1",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToAddress2",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToCity",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToPhoneNumber",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToStateProvince",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToZipPostalCode",
                table: "SaleOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerNo",
                table: "SaleOrders",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress1",
                table: "SaleOrders",
                type: "character varying(550)",
                maxLength: 550,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToAddress2",
                table: "SaleOrders",
                type: "character varying(550)",
                maxLength: 550,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToCity",
                table: "SaleOrders",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToPhoneNumber",
                table: "SaleOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipToStateProvince",
                table: "SaleOrders",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToZipPostalCode",
                table: "SaleOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
