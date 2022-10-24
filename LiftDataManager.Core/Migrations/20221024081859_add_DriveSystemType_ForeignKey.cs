using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_DriveSystemType_ForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsGearbox",
                table: "DriveSystems",
                newName: "DriveSystemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DriveSystems_DriveSystemTypeId",
                table: "DriveSystems",
                column: "DriveSystemTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriveSystems_DriveSystemTypes_DriveSystemTypeId",
                table: "DriveSystems",
                column: "DriveSystemTypeId",
                principalTable: "DriveSystemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriveSystems_DriveSystemTypes_DriveSystemTypeId",
                table: "DriveSystems");

            migrationBuilder.DropIndex(
                name: "IX_DriveSystems_DriveSystemTypeId",
                table: "DriveSystems");

            migrationBuilder.RenameColumn(
                name: "DriveSystemTypeId",
                table: "DriveSystems",
                newName: "IsGearbox");
        }
    }
}
