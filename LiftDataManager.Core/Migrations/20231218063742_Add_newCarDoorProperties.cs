using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_newCarDoorProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarDoorHeaderDepth",
                table: "CarDoors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarDoorHeaderHeight",
                table: "CarDoors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarDoorHeaderDepth",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "CarDoorHeaderHeight",
                table: "CarDoors");
        }
    }
}
