using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddTable_PutAway_PutAwayBin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingPutAway",
                table: "Receives");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Receives",
                newName: "Remaining");

            migrationBuilder.CreateTable(
                name: "PutAways",
                columns: table => new
                {
                    PutAwayId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ReceiveId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    PutAwayDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PutAways", x => x.PutAwayId);
                    table.ForeignKey(
                        name: "FK_PutAways_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_PutAways_Receives",
                        column: x => x.ReceiveId,
                        principalTable: "Receives",
                        principalColumn: "ReceiveId");
                });

            migrationBuilder.CreateTable(
                name: "PutAways_Bins",
                columns: table => new
                {
                    PutAwayBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    PutAwayId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PutAways_Bins", x => x.PutAwayBinId);
                    table.ForeignKey(
                        name: "FK_PutAwaysBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_PutAwaysBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_PutAwaysBins_PutAways",
                        column: x => x.PutAwayId,
                        principalTable: "PutAways",
                        principalColumn: "PutAwayId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_CustomerLocationId",
                table: "PutAways",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_ReceiveId",
                table: "PutAways",
                column: "ReceiveId");

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_Bins_BinId",
                table: "PutAways_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_Bins_CustomerLocationId",
                table: "PutAways_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_Bins_PutAwayId",
                table: "PutAways_Bins",
                column: "PutAwayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PutAways_Bins");

            migrationBuilder.DropTable(
                name: "PutAways");

            migrationBuilder.RenameColumn(
                name: "Remaining",
                table: "Receives",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "RemainingPutAway",
                table: "Receives",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
