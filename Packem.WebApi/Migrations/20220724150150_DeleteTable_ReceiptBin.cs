using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class DeleteTable_ReceiptBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Receipts_Bins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts_Bins",
                columns: table => new
                {
                    ReceiptBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ReceiptId = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts_Bins", x => x.ReceiptBinId);
                    table.ForeignKey(
                        name: "FK_ReceiptBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_ReceiptBins_Receipts",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "ReceiptId");
                    table.ForeignKey(
                        name: "FK_ReceiptsBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Bins_BinId",
                table: "Receipts_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Bins_CustomerLocationId",
                table: "Receipts_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Bins_Deleted",
                table: "Receipts_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Bins_ReceiptId",
                table: "Receipts_Bins",
                column: "ReceiptId");
        }
    }
}
