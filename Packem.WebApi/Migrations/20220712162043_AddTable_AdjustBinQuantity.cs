using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_AdjustBinQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdjustBinQuantities",
                columns: table => new
                {
                    AdjustBinQuantityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    OldQty = table.Column<int>(type: "integer", nullable: false),
                    NewQty = table.Column<int>(type: "integer", nullable: false),
                    AdjustDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjustBinQuantities", x => x.AdjustBinQuantityId);
                    table.ForeignKey(
                        name: "FK_AdjustBinQuantities_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_AdjustBinQuantities_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_AdjustBinQuantities_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdjustBinQuantities_BinId",
                table: "AdjustBinQuantities",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_AdjustBinQuantities_CustomerLocationId",
                table: "AdjustBinQuantities",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdjustBinQuantities_ItemId",
                table: "AdjustBinQuantities",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdjustBinQuantities");
        }
    }
}
