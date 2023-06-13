using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDoorManufacturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoorManufacturer",
                table: "LiftDoorGroups",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorManufacturer",
                table: "LiftDoorGroups");
        }
    }
}
