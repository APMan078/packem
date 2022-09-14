using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class Pallet_OneToMany_With_Bin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pallets_BinId",
                table: "Pallets");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_BinId",
                table: "Pallets",
                column: "BinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pallets_BinId",
                table: "Pallets");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_BinId",
                table: "Pallets",
                column: "BinId",
                unique: true);
        }
    }
}
