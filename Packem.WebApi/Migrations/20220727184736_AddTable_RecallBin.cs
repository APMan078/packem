using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_RecallBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recalls_Bins",
                columns: table => new
                {
                    RecallBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    RecallId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    PickDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recalls_Bins", x => x.RecallBinId);
                    table.ForeignKey(
                        name: "FK_RecallsBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_RecallsBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_RecallsBins_Recalls",
                        column: x => x.RecallId,
                        principalTable: "Recalls",
                        principalColumn: "RecallId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_Bins_BinId",
                table: "Recalls_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_Bins_CustomerLocationId",
                table: "Recalls_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_Bins_Deleted",
                table: "Recalls_Bins",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_Bins_RecallId",
                table: "Recalls_Bins",
                column: "RecallId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recalls_Bins");
        }
    }
}
