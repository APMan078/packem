using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_OrderLineBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderLines_Bins",
                columns: table => new
                {
                    OrderLineBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    OrderLineId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    PickDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines_Bins", x => x.OrderLineBinId);
                    table.ForeignKey(
                        name: "FK_OrderLinesBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_OrderLinesBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_OrderLinesBins_OrderLines",
                        column: x => x.OrderLineId,
                        principalTable: "OrderLines",
                        principalColumn: "OrderLineId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_Bins_BinId",
                table: "OrderLines_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_Bins_CustomerLocationId",
                table: "OrderLines_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_Bins_Deleted",
                table: "OrderLines_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_Bins_OrderLineId",
                table: "OrderLines_Bins",
                column: "OrderLineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLines_Bins");
        }
    }
}
