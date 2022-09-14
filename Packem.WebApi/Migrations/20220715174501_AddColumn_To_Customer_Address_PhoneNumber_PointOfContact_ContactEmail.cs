using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumn_To_Customer_Address_PhoneNumber_PointOfContact_ContactEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Customers",
                type: "character varying(550)",
                maxLength: 550,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Customers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PointOfContact",
                table: "Customers",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_Deleted",
                table: "Zones",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Deleted",
                table: "Vendors",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Deleted",
                table: "Users",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Zones_Bins_Deleted",
                table: "Transfers_Zones_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Deleted",
                table: "Transfers",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransferNews_Deleted",
                table: "TransferNews",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCurrents_Deleted",
                table: "TransferCurrents",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Deleted",
                table: "Roles",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Receives_Deleted",
                table: "Receives",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Bins_Deleted",
                table: "Receipts_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Deleted",
                table: "Receipts",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_Bins_Deleted",
                table: "PutAways_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_Deleted",
                table: "PutAways",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Deleted",
                table: "PurchaseOrders",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Vendors_Deleted",
                table: "Items_Vendors",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Deleted",
                table: "Items",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Zones_Deleted",
                table: "Inventories_Zones",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Bins_Deleted",
                table: "Inventories_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Deleted",
                table: "Inventories",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_Deleted",
                table: "ErrorLogs",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Deleted",
                table: "Customers",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLocations_Deleted",
                table: "CustomerLocations",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFacilities_Deleted",
                table: "CustomerFacilities",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDeviceTokens_Deleted",
                table: "CustomerDeviceTokens",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDevices_Deleted",
                table: "CustomerDevices",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Bins_Deleted",
                table: "Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_AdjustBinQuantities_Deleted",
                table: "AdjustBinQuantities",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Deleted",
                table: "ActivityLogs",
                column: "Deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Zones_Deleted",
                table: "Zones");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_Deleted",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Users_Deleted",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_Zones_Bins_Deleted",
                table: "Transfers_Zones_Bins");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_Deleted",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_TransferNews_Deleted",
                table: "TransferNews");

            migrationBuilder.DropIndex(
                name: "IX_TransferCurrents_Deleted",
                table: "TransferCurrents");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Deleted",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Receives_Deleted",
                table: "Receives");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_Bins_Deleted",
                table: "Receipts_Bins");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_Deleted",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_PutAways_Bins_Deleted",
                table: "PutAways_Bins");

            migrationBuilder.DropIndex(
                name: "IX_PutAways_Deleted",
                table: "PutAways");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_Deleted",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Items_Vendors_Deleted",
                table: "Items_Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Items_Deleted",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Zones_Deleted",
                table: "Inventories_Zones");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Bins_Deleted",
                table: "Inventories_Bins");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Deleted",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_ErrorLogs_Deleted",
                table: "ErrorLogs");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Deleted",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerLocations_Deleted",
                table: "CustomerLocations");

            migrationBuilder.DropIndex(
                name: "IX_CustomerFacilities_Deleted",
                table: "CustomerFacilities");

            migrationBuilder.DropIndex(
                name: "IX_CustomerDeviceTokens_Deleted",
                table: "CustomerDeviceTokens");

            migrationBuilder.DropIndex(
                name: "IX_CustomerDevices_Deleted",
                table: "CustomerDevices");

            migrationBuilder.DropIndex(
                name: "IX_Bins_Deleted",
                table: "Bins");

            migrationBuilder.DropIndex(
                name: "IX_AdjustBinQuantities_Deleted",
                table: "AdjustBinQuantities");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_Deleted",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PointOfContact",
                table: "Customers");
        }
    }
}
