using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_Transfer_TransferCurrent_TransferNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferCurrents",
                columns: table => new
                {
                    TransferCurrentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    CurrentZoneId = table.Column<int>(type: "integer", nullable: true),
                    CurrentBinId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferCurrents", x => x.TransferCurrentId);
                    table.ForeignKey(
                        name: "FK_TransferCurrents_CurrentBins",
                        column: x => x.CurrentBinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_TransferCurrents_CurrentZones",
                        column: x => x.CurrentZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId");
                    table.ForeignKey(
                        name: "FK_TransferCurrents_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateTable(
                name: "TransferNews",
                columns: table => new
                {
                    TransferNewId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    NewZoneId = table.Column<int>(type: "integer", nullable: true),
                    NewBinId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferNews", x => x.TransferNewId);
                    table.ForeignKey(
                        name: "FK_TransferNews_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_TransferNews_NewBins",
                        column: x => x.NewBinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_TransferNews_NewZones",
                        column: x => x.NewZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId");
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    TransferId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    TransferCurrentId = table.Column<int>(type: "integer", nullable: true),
                    TransferNewId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TransferDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.TransferId);
                    table.ForeignKey(
                        name: "FK_Transfers_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_Transfers_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_Transfers_TransferCurrents",
                        column: x => x.TransferCurrentId,
                        principalTable: "TransferCurrents",
                        principalColumn: "TransferCurrentId");
                    table.ForeignKey(
                        name: "FK_Transfers_TransferNews",
                        column: x => x.TransferNewId,
                        principalTable: "TransferNews",
                        principalColumn: "TransferNewId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferCurrents_CurrentBinId",
                table: "TransferCurrents",
                column: "CurrentBinId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCurrents_CurrentZoneId",
                table: "TransferCurrents",
                column: "CurrentZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferCurrents_CustomerLocationId",
                table: "TransferCurrents",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferNews_CustomerLocationId",
                table: "TransferNews",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferNews_NewBinId",
                table: "TransferNews",
                column: "NewBinId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferNews_NewZoneId",
                table: "TransferNews",
                column: "NewZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_CustomerLocationId",
                table: "Transfers",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ItemId",
                table: "Transfers",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferCurrentId",
                table: "Transfers",
                column: "TransferCurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TransferNewId",
                table: "Transfers",
                column: "TransferNewId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "TransferCurrents");

            migrationBuilder.DropTable(
                name: "TransferNews");
        }
    }
}
