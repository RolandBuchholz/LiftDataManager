using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_Profil_RadiusOfInertia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RadiusOfInertiaX",
                table: "BufferPropProfiles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RadiusOfInertiaY",
                table: "BufferPropProfiles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RadiusOfInertiaX",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "RadiusOfInertiaY",
                table: "BufferPropProfiles");
        }
    }
}
