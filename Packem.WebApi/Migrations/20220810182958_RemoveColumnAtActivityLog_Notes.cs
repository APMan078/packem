using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class RemoveColumnAtActivityLog_Notes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ActivityLogs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ActivityLogs",
                type: "character varying(550)",
                maxLength: 550,
                nullable: true);
        }
    }
}
