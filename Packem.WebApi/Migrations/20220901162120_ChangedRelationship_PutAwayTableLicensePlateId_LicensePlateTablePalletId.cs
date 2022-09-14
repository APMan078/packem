using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class ChangedRelationship_PutAwayTableLicensePlateId_LicensePlateTablePalletId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pallets_LicensePlates",
                table: "Pallets");

            migrationBuilder.DropForeignKey(
                name: "FK_PutAways_Pallets",
                table: "PutAways");

            migrationBuilder.DropIndex(
                name: "IX_Pallets_LicensePlateId",
                table: "Pallets");

            migrationBuilder.DropColumn(
                name: "LicensePlateId",
                table: "Pallets");

            migrationBuilder.RenameColumn(
                name: "PalletId",
                table: "PutAways",
                newName: "LicensePlateId");

            migrationBuilder.RenameIndex(
                name: "IX_PutAways_PalletId",
                table: "PutAways",
                newName: "IX_PutAways_LicensePlateId");

            migrationBuilder.AddColumn<int>(
                name: "PalletId",
                table: "LicensePlates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlates_PalletId",
                table: "LicensePlates",
                column: "PalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_LicensePlates_Pallets",
                table: "LicensePlates",
                column: "PalletId",
                principalTable: "Pallets",
                principalColumn: "PalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_PutAways_LicensePlates",
                table: "PutAways",
                column: "LicensePlateId",
                principalTable: "LicensePlates",
                principalColumn: "LicensePlateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicensePlates_Pallets",
                table: "LicensePlates");

            migrationBuilder.DropForeignKey(
                name: "FK_PutAways_LicensePlates",
                table: "PutAways");

            migrationBuilder.DropIndex(
                name: "IX_LicensePlates_PalletId",
                table: "LicensePlates");

            migrationBuilder.DropColumn(
                name: "PalletId",
                table: "LicensePlates");

            migrationBuilder.RenameColumn(
                name: "LicensePlateId",
                table: "PutAways",
                newName: "PalletId");

            migrationBuilder.RenameIndex(
                name: "IX_PutAways_LicensePlateId",
                table: "PutAways",
                newName: "IX_PutAways_PalletId");

            migrationBuilder.AddColumn<int>(
                name: "LicensePlateId",
                table: "Pallets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_LicensePlateId",
                table: "Pallets",
                column: "LicensePlateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pallets_LicensePlates",
                table: "Pallets",
                column: "LicensePlateId",
                principalTable: "LicensePlates",
                principalColumn: "LicensePlateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PutAways_Pallets",
                table: "PutAways",
                column: "PalletId",
                principalTable: "Pallets",
                principalColumn: "PalletId");
        }
    }
}
