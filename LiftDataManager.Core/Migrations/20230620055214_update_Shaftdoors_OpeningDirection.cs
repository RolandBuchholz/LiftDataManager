using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class update_Shaftdoors_OpeningDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShaftDoors_LiftDoorOpeningDirectionId",
                table: "ShaftDoors",
                column: "LiftDoorOpeningDirectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShaftDoors_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "ShaftDoors",
                column: "LiftDoorOpeningDirectionId",
                principalTable: "LiftDoorOpeningDirections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShaftDoors_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "ShaftDoors");

            migrationBuilder.DropIndex(
                name: "IX_ShaftDoors_LiftDoorOpeningDirectionId",
                table: "ShaftDoors");
        }
    }
}
