using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_DoorsForeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarDoorId",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShaftDoorId",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LiftDoorGroups_CarDoorId",
                table: "LiftDoorGroups",
                column: "CarDoorId");

            migrationBuilder.CreateIndex(
                name: "IX_LiftDoorGroups_ShaftDoorId",
                table: "LiftDoorGroups",
                column: "ShaftDoorId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftDoorGroups_CarDoors_CarDoorId",
                table: "LiftDoorGroups",
                column: "CarDoorId",
                principalTable: "CarDoors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LiftDoorGroups_ShaftDoors_ShaftDoorId",
                table: "LiftDoorGroups",
                column: "ShaftDoorId",
                principalTable: "ShaftDoors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftDoorGroups_CarDoors_CarDoorId",
                table: "LiftDoorGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_LiftDoorGroups_ShaftDoors_ShaftDoorId",
                table: "LiftDoorGroups");

            migrationBuilder.DropIndex(
                name: "IX_LiftDoorGroups_CarDoorId",
                table: "LiftDoorGroups");

            migrationBuilder.DropIndex(
                name: "IX_LiftDoorGroups_ShaftDoorId",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "CarDoorId",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "ShaftDoorId",
                table: "LiftDoorGroups");
        }
    }
}
