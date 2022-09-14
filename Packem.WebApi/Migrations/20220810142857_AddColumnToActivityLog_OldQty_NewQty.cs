using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packem.WebApi.Migrations
{
    public partial class AddColumnToActivityLog_OldQty_NewQty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivityDate",
                table: "ActivityLogs",
                newName: "ActivityDateTime");

            migrationBuilder.AddColumn<int>(
                name: "NewQty",
                table: "ActivityLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OldQty",
                table: "ActivityLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewQty",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "OldQty",
                table: "ActivityLogs");

            migrationBuilder.RenameColumn(
                name: "ActivityDateTime",
                table: "ActivityLogs",
                newName: "ActivityDate");
        }
    }
}
