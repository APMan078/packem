using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class CreateTable_UnitOfMeasure_And_UnitOfMeasureCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitOfMeasures",
                columns: table => new
                {
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasures", x => x.UnitOfMeasureId);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasures_Customers",
                columns: table => new
                {
                    UnitOfMeasureCustomerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UnitOfMeasureId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasures_Customers", x => x.UnitOfMeasureCustomerId);
                    table.ForeignKey(
                        name: "FK_UnitOfMeasureCustomers_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_UnitOfMeasureCustomers_UnitOfMeasures",
                        column: x => x.UnitOfMeasureId,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "UnitOfMeasureId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Deleted",
                table: "UnitOfMeasures",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Customers_CustomerId",
                table: "UnitOfMeasures_Customers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Customers_Deleted",
                table: "UnitOfMeasures_Customers",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Customers_UnitOfMeasureId",
                table: "UnitOfMeasures_Customers",
                column: "UnitOfMeasureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitOfMeasures_Customers");

            migrationBuilder.DropTable(
                name: "UnitOfMeasures");
        }
    }
}
