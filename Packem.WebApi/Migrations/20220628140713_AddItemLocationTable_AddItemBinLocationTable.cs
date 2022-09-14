using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddItemLocationTable_AddItemBinLocationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Bins",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Locations",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_BinId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_LocationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "BinId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Items",
                type: "character varying(55)",
                maxLength: 55,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseLocation",
                table: "CustomerFacilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Bins",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemBins",
                columns: table => new
                {
                    ItemBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBins", x => x.ItemBinId);
                    table.ForeignKey(
                        name: "FK_ItemBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_ItemBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_ItemBins_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "ItemLocations",
                columns: table => new
                {
                    ItemLocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLocations", x => x.ItemLocationId);
                    table.ForeignKey(
                        name: "FK_ItemLocations_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_ItemLocations_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_Receipts_Locations",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bins_LocationId",
                table: "Bins",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBins_BinId",
                table: "ItemBins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBins_CustomerLocationId",
                table: "ItemBins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBins_ItemId",
                table: "ItemBins",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocations_CustomerLocationId",
                table: "ItemLocations",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocations_ItemId",
                table: "ItemLocations",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLocations_LocationId",
                table: "ItemLocations",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Locations",
                table: "Bins",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Locations",
                table: "Bins");

            migrationBuilder.DropTable(
                name: "ItemBins");

            migrationBuilder.DropTable(
                name: "ItemLocations");

            migrationBuilder.DropIndex(
                name: "IX_Bins_LocationId",
                table: "Bins");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UseLocation",
                table: "CustomerFacilities");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Bins");

            migrationBuilder.AddColumn<int>(
                name: "BinId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_BinId",
                table: "Items",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_LocationId",
                table: "Items",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Bins",
                table: "Items",
                column: "BinId",
                principalTable: "Bins",
                principalColumn: "BinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Locations",
                table: "Items",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId");
        }
    }
}
