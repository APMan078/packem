using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddCustomerFacilityIdAtItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerFacilityId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CustomerFacilityId",
                table: "Items",
                column: "CustomerFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_CustomerFacilities",
                table: "Items",
                column: "CustomerFacilityId",
                principalTable: "CustomerFacilities",
                principalColumn: "CustomerFacilityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_CustomerFacilities",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CustomerFacilityId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CustomerFacilityId",
                table: "Items");
        }
    }
}
