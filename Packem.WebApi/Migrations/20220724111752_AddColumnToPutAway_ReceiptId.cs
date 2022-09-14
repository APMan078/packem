using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumnToPutAway_ReceiptId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PutAwayType",
                table: "PutAways",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptId",
                table: "PutAways",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_ReceiptId",
                table: "PutAways",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_PutAways_Receipts",
                table: "PutAways",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "ReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PutAways_Receipts",
                table: "PutAways");

            migrationBuilder.DropIndex(
                name: "IX_PutAways_ReceiptId",
                table: "PutAways");

            migrationBuilder.DropColumn(
                name: "PutAwayType",
                table: "PutAways");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "PutAways");
        }
    }
}
