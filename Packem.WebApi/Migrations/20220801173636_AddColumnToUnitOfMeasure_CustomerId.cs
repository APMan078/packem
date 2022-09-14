using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumnToUnitOfMeasure_CustomerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "UnitOfMeasures",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_CustomerId",
                table: "UnitOfMeasures",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitOfMeasures_Customers",
                table: "UnitOfMeasures",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnitOfMeasures_Customers",
                table: "UnitOfMeasures");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasures_CustomerId",
                table: "UnitOfMeasures");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "UnitOfMeasures");
        }
    }
}
