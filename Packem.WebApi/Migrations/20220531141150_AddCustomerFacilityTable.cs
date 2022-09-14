using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddCustomerFacilityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerFacilities",
                columns: table => new
                {
                    CustomerFacilityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerLocationId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerFacilities", x => x.CustomerFacilityId);
                    table.ForeignKey(
                        name: "FK_CustomerFacilities_CustomerLocations",
                        column: x => x.CustomerLocationId,
                        principalTable: "CustomerLocations",
                        principalColumn: "CustomerLocationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFacilities_CustomerLocationId",
                table: "CustomerFacilities",
                column: "CustomerLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerFacilities");
        }
    }
}
