using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_DriveTypeForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveType",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "DriveType",
                table: "CarFrameTypes");

            migrationBuilder.AddColumn<int>(
                name: "DriveTypeId",
                table: "LiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DriveTypeId",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LiftTypes_DriveTypeId",
                table: "LiftTypes",
                column: "DriveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarFrameTypes_DriveTypeId",
                table: "CarFrameTypes",
                column: "DriveTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarFrameTypes_DriveTypes_DriveTypeId",
                table: "CarFrameTypes",
                column: "DriveTypeId",
                principalTable: "DriveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LiftTypes_DriveTypes_DriveTypeId",
                table: "LiftTypes",
                column: "DriveTypeId",
                principalTable: "DriveTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarFrameTypes_DriveTypes_DriveTypeId",
                table: "CarFrameTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_LiftTypes_DriveTypes_DriveTypeId",
                table: "LiftTypes");

            migrationBuilder.DropIndex(
                name: "IX_LiftTypes_DriveTypeId",
                table: "LiftTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarFrameTypes_DriveTypeId",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "DriveTypeId",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "DriveTypeId",
                table: "CarFrameTypes");

            migrationBuilder.AddColumn<string>(
                name: "DriveType",
                table: "LiftTypes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DriveType",
                table: "CarFrameTypes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
