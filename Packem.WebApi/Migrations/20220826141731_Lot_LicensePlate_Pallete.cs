using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class Lot_LicensePlate_Pallete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "Receives",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "Receipts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "Inventories_Bins",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Bins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LicensePlates",
                columns: table => new
                {
                    LicensePlateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    LicensePlateNo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LicensePlateType = table.Column<int>(type: "integer", nullable: false),
                    ArrivalDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Printed = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePlates", x => x.LicensePlateId);
                    table.ForeignKey(
                        name: "FK_LicensePlates_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_LicensePlates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    LotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    LotNo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.LotId);
                    table.ForeignKey(
                        name: "FK_Lots_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_Lots_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                });

            migrationBuilder.CreateTable(
                name: "Pallets",
                columns: table => new
                {
                    PalletId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
                    LicensePlateId = table.Column<int>(type: "integer", nullable: true),
                    BinId = table.Column<int>(type: "integer", nullable: true),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pallets", x => x.PalletId);
                    table.ForeignKey(
                        name: "FK_Pallets_Bins",
                        column: x => x.BinId,
                        principalTable: "Bins",
                        principalColumn: "BinId");
                    table.ForeignKey(
                        name: "FK_Pallets_CustomerFacilities",
                        column: x => x.CustomerFacilityId,
                        principalTable: "CustomerFacilities",
                        principalColumn: "CustomerFacilityId");
                    table.ForeignKey(
                        name: "FK_Pallets_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_Pallets_LicensePlates",
                        column: x => x.LicensePlateId,
                        principalTable: "LicensePlates",
                        principalColumn: "LicensePlateId");
                });

            migrationBuilder.CreateTable(
                name: "LicensePlates_Items",
                columns: table => new
                {
                    LicensePlateItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    LicensePlateId = table.Column<int>(type: "integer", nullable: true),
                    ItemId = table.Column<int>(type: "integer", nullable: true),
                    VendorId = table.Column<int>(type: "integer", nullable: true),
                    LotId = table.Column<int>(type: "integer", nullable: true),
                    ReferenceNo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Cases = table.Column<int>(type: "integer", nullable: true),
                    EaCase = table.Column<int>(type: "integer", nullable: true),
                    TotalQty = table.Column<int>(type: "integer", nullable: false),
                    TotalWeight = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePlates_Items", x => x.LicensePlateItemId);
                    table.ForeignKey(
                        name: "FK_LicensePlatesItems_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_LicensePlatesItems_Items",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId");
                    table.ForeignKey(
                        name: "FK_LicensePlatesItems_LicensePlates",
                        column: x => x.LicensePlateId,
                        principalTable: "LicensePlates",
                        principalColumn: "LicensePlateId");
                    table.ForeignKey(
                        name: "FK_LicensePlatesItems_Lots",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "LotId");
                    table.ForeignKey(
                        name: "FK_LicensePlatesItems_Vendors",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId");
                });

            migrationBuilder.CreateTable(
                name: "Pallets_Inventories",
                columns: table => new
                {
                    PalletInventoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: true),
                    PalletId = table.Column<int>(type: "integer", nullable: true),
                    InventoryId = table.Column<int>(type: "integer", nullable: true),
                    LotId = table.Column<int>(type: "integer", nullable: true),
                    UOM = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    UOMQty = table.Column<int>(type: "integer", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pallets_Inventories", x => x.PalletInventoryId);
                    table.ForeignKey(
                        name: "FK_PalletsInventories_CustomerFacilities",
                        column: x => x.CustomerFacilityId,
                        principalTable: "CustomerFacilities",
                        principalColumn: "CustomerFacilityId");
                    table.ForeignKey(
                        name: "FK_PalletsInventories_Customers",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                    table.ForeignKey(
                        name: "FK_PalletsInventories_Inventories",
                        column: x => x.InventoryId,
                        principalTable: "Inventories",
                        principalColumn: "InventoryId");
                    table.ForeignKey(
                        name: "FK_PalletsInventories_Lots",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "LotId");
                    table.ForeignKey(
                        name: "FK_PalletsInventories_Pallets",
                        column: x => x.PalletId,
                        principalTable: "Pallets",
                        principalColumn: "PalletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receives_LotId",
                table: "Receives",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_LotId",
                table: "Receipts",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Bins_LotId",
                table: "Inventories_Bins",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_CustomerId",
                table: "LicensePlates",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Deleted",
                table: "LicensePlates",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_UserId",
                table: "LicensePlates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Items_CustomerId",
                table: "LicensePlates_Items",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Items_Deleted",
                table: "LicensePlates_Items",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Items_ItemId",
                table: "LicensePlates_Items",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Items_LicensePlateId",
                table: "LicensePlates_Items",
                column: "LicensePlateId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Items_LotId",
                table: "LicensePlates_Items",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_Items_VendorId",
                table: "LicensePlates_Items",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_CustomerLocationId",
                table: "Lots",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_Deleted",
                table: "Lots",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ItemId",
                table: "Lots",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_BinId",
                table: "Pallets",
                column: "BinId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_CustomerFacilityId",
                table: "Pallets",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_CustomerLocationId",
                table: "Pallets",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Deleted",
                table: "Pallets",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_LicensePlateId",
                table: "Pallets",
                column: "LicensePlateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_CustomerFacilityId",
                table: "Pallets_Inventories",
                column: "CustomerFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_CustomerLocationId",
                table: "Pallets_Inventories",
                column: "CustomerLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_Deleted",
                table: "Pallets_Inventories",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_InventoryId",
                table: "Pallets_Inventories",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_LotId",
                table: "Pallets_Inventories",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_PalletId",
                table: "Pallets_Inventories",
                column: "PalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoriesBins_Lots",
                table: "Inventories_Bins",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Lots",
                table: "Receipts",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receives_Lots",
                table: "Receives",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "LotId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoriesBins_Lots",
                table: "Inventories_Bins");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Lots",
                table: "Receipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Receives_Lots",
                table: "Receives");

            migrationBuilder.DropTable(
                name: "LicensePlates_Items");

            migrationBuilder.DropTable(
                name: "Pallets_Inventories");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Pallets");

            migrationBuilder.DropTable(
                name: "LicensePlates");

            migrationBuilder.DropIndex(
                name: "IX_Receives_LotId",
                table: "Receives");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_LotId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_Bins_LotId",
                table: "Inventories_Bins");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "Receives");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "Inventories_Bins");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Bins");
        }
    }
}
