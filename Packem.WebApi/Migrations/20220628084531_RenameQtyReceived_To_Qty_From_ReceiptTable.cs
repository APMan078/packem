using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class RenameQtyReceived_To_Qty_From_ReceiptTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QtyReceived",
                table: "Receipts",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "Receipts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Qty",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Receipts",
                newName: "QtyReceived");
        }
    }
}
