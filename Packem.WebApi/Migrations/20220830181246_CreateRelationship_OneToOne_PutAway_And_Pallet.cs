using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class CreateRelationship_OneToOne_PutAway_And_Pallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PalletId",
                table: "PutAways",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PutAways_PalletId",
                table: "PutAways",
                column: "PalletId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PutAways_Pallets",
                table: "PutAways",
                column: "PalletId",
                principalTable: "Pallets",
                principalColumn: "PalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PutAways_Pallets",
                table: "PutAways");

            migrationBuilder.DropIndex(
                name: "IX_PutAways_PalletId",
                table: "PutAways");

            migrationBuilder.DropColumn(
                name: "PalletId",
                table: "PutAways");
        }
    }
}
