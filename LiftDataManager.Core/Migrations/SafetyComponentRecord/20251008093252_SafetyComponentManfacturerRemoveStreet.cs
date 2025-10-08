using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations.SafetyComponentRecord
{
    /// <inheritdoc />
    public partial class SafetyComponentManfacturerRemoveStreet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "SafetyComponentManfacturers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "SafetyComponentManfacturers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "SafetyComponentManfacturers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "SafetyComponentManfacturers",
                type: "TEXT",
                nullable: true);
        }
    }
}
