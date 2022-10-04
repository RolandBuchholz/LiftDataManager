using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class carFrameType_DriveType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriveTyp",
                table: "CarFrameTypes",
                newName: "DriveType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriveType",
                table: "CarFrameTypes",
                newName: "DriveTyp");
        }
    }
}
