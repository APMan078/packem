using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddInventoryTable_AddInventoryLocationTable_AddInventoryBinTable_RemoveItemLocationTable_RemoveItemBinTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemBins");

            migrationBuilder.DropTable(
                name: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "QtyOnHand",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "ItemNo",
                table: "Items",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "integer", nullable: false),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    QtyOnHand = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventories_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_Inventories_Items",
                        column: x => x.InventoryId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "Inventories_Bins",
                columns: table => new
                {
                    InventoryBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories_Bins", x => x.InventoryBinId);
                    table.ForeignKey(
                        name: "FK_InventoriesBins_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_InventoriesBins_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_InventoriesBins_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                });

            migrationBuilder.CreateTable(
                name: "Inventories_Locations",
                columns: table => new
                {
                    InventoryLocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories_Locations", x => x.InventoryLocationId);
                    table.ForeignKey(
                        name: "FK_InventoriesLocations_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_InventoriesLocations_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                    table.ForeignKey(
                        name: "FK_InventoriesLocations_Locations",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CustomerLocationId",
                table: "Inventories",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Bins_BinId",
                table: "Inventories_Bins",
                column: "BinId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Bins_CustomerLocationId",
                table: "Inventories_Bins",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Bins_InventoryId",
                table: "Inventories_Bins",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Locations_CustomerLocationId",
                table: "Inventories_Locations",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Locations_InventoryId",
                table: "Inventories_Locations",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Locations_LocationId",
                table: "Inventories_Locations",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories_Bins");

            migrationBuilder.DropTable(
                name: "Inventories_Locations");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.AlterColumn<int>(
                name: "ItemNo",
                table: "Items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AddColumn<int>(
                name: "QtyOnHand",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemBins",
                columns: table => new
                {
                    ItemBinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
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
                    LocationId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false)
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
        }
    }
}
