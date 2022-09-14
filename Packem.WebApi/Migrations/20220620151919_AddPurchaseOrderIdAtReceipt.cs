using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddPurchaseOrderIdAtReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Receipts",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_ReceiptId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "PurchaseOrders");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                table: "Receipts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PurchaseOrderId",
                table: "Receipts",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_PurchaseOrders",
                table: "Receipts",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_PurchaseOrders",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_PurchaseOrderId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "Receipts");

            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "PurchaseOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_ReceiptId",
                table: "PurchaseOrders",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Receipts",
                table: "PurchaseOrders",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "ReceiptId");
        }
    }
}
