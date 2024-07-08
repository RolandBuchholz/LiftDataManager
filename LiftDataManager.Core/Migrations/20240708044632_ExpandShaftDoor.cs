using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class ExpandShaftDoor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DefaultFrameDepth",
                table: "ShaftDoors",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DefaultFrameWidth",
                table: "ShaftDoors",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DoorPanelSpace",
                table: "ShaftDoors",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DoorPanelWidth",
                table: "ShaftDoors",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SillWidth",
                table: "ShaftDoors",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultFrameDepth",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "DefaultFrameWidth",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "DoorPanelSpace",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "DoorPanelWidth",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "SillWidth",
                table: "ShaftDoors");
        }
    }
}
