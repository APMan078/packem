using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumn_ShipVia_At_PurchaseOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Accepted",
                table: "Receives",
                newName: "Received");

            migrationBuilder.AddColumn<string>(
                name: "ShipVia",
                table: "PurchaseOrders",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipVia",
                table: "PurchaseOrders");

            migrationBuilder.RenameColumn(
                name: "Received",
                table: "Receives",
                newName: "Accepted");
        }
    }
}
