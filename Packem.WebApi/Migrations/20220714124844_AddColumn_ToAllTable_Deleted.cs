using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumn_ToAllTable_Deleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Zones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Vendors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Transfers_Zones_Bins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Transfers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "TransferNews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "TransferCurrents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Receives",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Receipts_Bins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Receipts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "PutAways_Bins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "PutAways",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Items_Vendors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Inventories_Zones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Inventories_Bins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Inventories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ErrorLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CustomerLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CustomerFacilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CustomerDeviceTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CustomerDevices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Bins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AdjustBinQuantities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ActivityLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Transfers_Zones_Bins");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "TransferNews");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "TransferCurrents");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Receipts_Bins");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "PutAways_Bins");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "PutAways");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Items_Vendors");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Inventories_Zones");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Inventories_Bins");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CustomerLocations");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CustomerDeviceTokens");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CustomerDevices");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AdjustBinQuantities");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ActivityLogs");
        }
    }
}
