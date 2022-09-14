using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumn_Remaining_Rename_Column_ReceivedDateTime_To_CompletedDateTime_AddTable_TransferZoneBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReceivedDateTime",
                table: "Transfers",
                newName: "CompletedDateTime");

            migrationBuilder.AddColumn<int>(
                name: "Remaining",
                table: "Transfers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Transfers_Zones_Bins",
                columns: table => new
                {
                    TransferZoneBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    TransferId = table.Column<int>(type: "integer", nullable: true),
                    ZoneId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers_Zones_Bins", x => x.TransferZoneBinId);
                    table.ForeignKey(
                        name: "FK_TransferZoneBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_TransferZoneBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_TransferZoneBins_Transfers",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "TransferId");
                    table.ForeignKey(
                        name: "FK_TransferZoneBins_Zones",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Zones_Bins_BinId",
                table: "Transfers_Zones_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Zones_Bins_CustomerLocationId",
                table: "Transfers_Zones_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Zones_Bins_TransferId",
                table: "Transfers_Zones_Bins",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Zones_Bins_ZoneId",
                table: "Transfers_Zones_Bins",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers_Zones_Bins");

            migrationBuilder.DropColumn(
                name: "Remaining",
                table: "Transfers");

            migrationBuilder.RenameColumn(
                name: "CompletedDateTime",
                table: "Transfers",
                newName: "ReceivedDateTime");
        }
    }
}
