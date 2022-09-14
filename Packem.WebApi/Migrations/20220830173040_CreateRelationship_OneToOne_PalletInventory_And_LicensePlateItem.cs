using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class CreateRelationship_OneToOne_PalletInventory_And_LicensePlateItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PalletsInventories_Lots",
                table: "Pallets_Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_Inventories_LotId",
                table: "Pallets_Inventories");

            migrationBuilder.DropColumn(
                name: "UOM",
                table: "Pallets_Inventories");

            migrationBuilder.DropColumn(
                name: "UOMQty",
                table: "Pallets_Inventories");

            migrationBuilder.RenameColumn(
                name: "LotId",
                table: "Pallets_Inventories",
                newName: "LicensePlateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_LicensePlateItemId",
                table: "Pallets_Inventories",
                column: "LicensePlateItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PalletsInventories_LicensePlatesItems",
                table: "Pallets_Inventories",
                column: "LicensePlateItemId",
                principalTable: "LicensePlates_Items",
                principalColumn: "LicensePlateItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PalletsInventories_LicensePlatesItems",
                table: "Pallets_Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_Inventories_LicensePlateItemId",
                table: "Pallets_Inventories");

            migrationBuilder.RenameColumn(
                name: "LicensePlateItemId",
                table: "Pallets_Inventories",
                newName: "LotId");

            migrationBuilder.AddColumn<string>(
                name: "UOM",
                table: "Pallets_Inventories",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UOMQty",
                table: "Pallets_Inventories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_Inventories_LotId",
                table: "Pallets_Inventories",
                column: "LotId");

            migrationBuilder.AddForeignKey(
                name: "FK_PalletsInventories_Lots",
                table: "Pallets_Inventories",
                column: "LotId",
                principalTable: "Lots",
                principalColumn: "LotId");
        }
    }
}
