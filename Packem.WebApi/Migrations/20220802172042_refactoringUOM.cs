using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class refactoringUOM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UOM",
                table: "Items");

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ExpirationDate",
            //    table: "Items",
            //    type: "timestamp without time zone",
            //    nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitOfMeasureId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UnitOfMeasureId",
                table: "Items",
                column: "UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_UnitOfMeasures",
                table: "Items",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "UnitOfMeasureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_UnitOfMeasures",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UnitOfMeasureId",
                table: "Items");

            //migrationBuilder.DropColumn(
            //    name: "ExpirationDate",
            //    table: "Items");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasureId",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "UOM",
                table: "Items",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
