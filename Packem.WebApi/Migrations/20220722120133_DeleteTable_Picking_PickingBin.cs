using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class DeleteTable_Picking_PickingBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pickings_Bins");

            migrationBuilder.DropTable(
                name: "Pickings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pickings",
                columns: table => new
                {
                    PickingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    OrderLineId = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    PickingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    Remaining = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pickings", x => x.PickingId);
                    table.ForeignKey(
                        name: "FK_Pickings_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_Pickings_OrderLines",
                        column: x => x.OrderLineId,
                        principalTable: "OrderLines",
                        principalColumn: "OrderLineId");
                });

            migrationBuilder.CreateTable(
                name: "Pickings_Bins",
                columns: table => new
                {
                    PickingBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    PickingId = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pickings_Bins", x => x.PickingBinId);
                    table.ForeignKey(
                        name: "FK_PickingsBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_PickingsBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_PickingsBins_Pickings",
                        column: x => x.PickingId,
                        principalTable: "Pickings",
                        principalColumn: "PickingId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_CustomerLocationId",
                table: "Pickings",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_Deleted",
                table: "Pickings",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_OrderLineId",
                table: "Pickings",
                column: "OrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_Bins_BinId",
                table: "Pickings_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_Bins_CustomerLocationId",
                table: "Pickings_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_Bins_Deleted",
                table: "Pickings_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Pickings_Bins_PickingId",
                table: "Pickings_Bins",
                column: "PickingId");
        }
    }
}
