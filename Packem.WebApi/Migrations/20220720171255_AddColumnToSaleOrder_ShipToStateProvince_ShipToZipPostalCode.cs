using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumnToSaleOrder_ShipToStateProvince_ShipToZipPostalCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipToStateProvince",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "ShipToZipPostalCode",
                table: "SaleOrders");
        }
    }
}
