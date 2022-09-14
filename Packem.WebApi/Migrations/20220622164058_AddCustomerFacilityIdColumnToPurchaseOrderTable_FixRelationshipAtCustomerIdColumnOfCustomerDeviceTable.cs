using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddCustomerFacilityIdColumnToPurchaseOrderTable_FixRelationshipAtCustomerIdColumnOfCustomerDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerFacilityId",
                table: "PurchaseOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustomerDevices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CustomerFacilityId",
                table: "PurchaseOrders",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDevices_CustomerId",
                table: "CustomerDevices",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerDevices_Customers",
                table: "CustomerDevices",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_CustomerFacilities",
                table: "PurchaseOrders",
                column: "CustomerFacilityId",
                principalTable: "CustomerFacilities",
                principalColumn: "CustomerFacilityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerDevices_Customers",
                table: "CustomerDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_CustomerFacilities",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_CustomerFacilityId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerDevices_CustomerId",
                table: "CustomerDevices");

            migrationBuilder.DropColumn(
                name: "CustomerFacilityId",
                table: "PurchaseOrders");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "CustomerDevices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
