using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class ProfileDetailData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AreaOfProfile",
                table: "BufferPropProfiles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MomentOfInertiaX",
                table: "BufferPropProfiles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MomentOfInertiaY",
                table: "BufferPropProfiles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaOfProfile",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "MomentOfInertiaX",
                table: "BufferPropProfiles");

            migrationBuilder.DropColumn(
                name: "MomentOfInertiaY",
                table: "BufferPropProfiles");
        }
    }
}
