using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_SafetyGearForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SafetyGearTypeId",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyGearModelTypes_SafetyGearTypeId",
                table: "SafetyGearModelTypes",
                column: "SafetyGearTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SafetyGearModelTypes_SafetyGearTypes_SafetyGearTypeId",
                table: "SafetyGearModelTypes",
                column: "SafetyGearTypeId",
                principalTable: "SafetyGearTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SafetyGearModelTypes_SafetyGearTypes_SafetyGearTypeId",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropIndex(
                name: "IX_SafetyGearModelTypes_SafetyGearTypeId",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "SafetyGearTypeId",
                table: "SafetyGearModelTypes");
        }
    }
}
