using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumnToRecall_CustomerFacilityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerFacilityId",
                table: "Recalls",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recalls_CustomerFacilityId",
                table: "Recalls",
                column: "CustomerFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recalls_CustomerFacilities",
                table: "Recalls",
                column: "CustomerFacilityId",
                principalTable: "CustomerFacilities",
                principalColumn: "CustomerFacilityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recalls_CustomerFacilities",
                table: "Recalls");

            migrationBuilder.DropIndex(
                name: "IX_Recalls_CustomerFacilityId",
                table: "Recalls");

            migrationBuilder.DropColumn(
                name: "CustomerFacilityId",
                table: "Recalls");
        }
    }
}
