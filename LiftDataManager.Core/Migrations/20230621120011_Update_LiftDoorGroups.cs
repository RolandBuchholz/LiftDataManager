using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update_LiftDoorGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "DoorPanelCount",
                table: "LiftDoorGroups");

            migrationBuilder.AlterColumn<int>(
                name: "LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                column: "LiftDoorOpeningDirectionId",
                principalTable: "LiftDoorOpeningDirections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.AlterColumn<int>(
                name: "LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoorPanelCount",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                column: "LiftDoorOpeningDirectionId",
                principalTable: "LiftDoorOpeningDirections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
