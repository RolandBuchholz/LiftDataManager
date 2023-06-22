using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update_CarDoorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.DropIndex(
                name: "IX_LiftDoorGroups_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.AddColumn<int>(
                name: "LiftDoorOpeningDirectionId",
                table: "CarDoors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarDoors_LiftDoorOpeningDirectionId",
                table: "CarDoors",
                column: "LiftDoorOpeningDirectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarDoors_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "CarDoors",
                column: "LiftDoorOpeningDirectionId",
                principalTable: "LiftDoorOpeningDirections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarDoors_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "CarDoors");

            migrationBuilder.DropIndex(
                name: "IX_CarDoors_LiftDoorOpeningDirectionId",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "LiftDoorOpeningDirectionId",
                table: "CarDoors");

            migrationBuilder.AddColumn<int>(
                name: "LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiftDoorGroups_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                column: "LiftDoorOpeningDirectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                column: "LiftDoorOpeningDirectionId",
                principalTable: "LiftDoorOpeningDirections",
                principalColumn: "Id");
        }
    }
}
