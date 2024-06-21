using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_GuideRailData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Area",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FlangeC",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ForgedClips",
                table: "GuideRailss",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ForgedClipsForce",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ModulusOfResistanceX",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ModulusOfResistanceY",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MomentOfInertiaX",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MomentOfInertiaY",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RadiusOfInertiaX",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RadiusOfInertiaY",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "SlidingClips",
                table: "GuideRailss",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SlidingClipsForce",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ThicknessF",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Width",
                table: "GuideRailss",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "FlangeC",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "ForgedClips",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "ForgedClipsForce",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "ModulusOfResistanceX",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "ModulusOfResistanceY",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "MomentOfInertiaX",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "MomentOfInertiaY",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "RadiusOfInertiaX",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "RadiusOfInertiaY",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "SlidingClips",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "SlidingClipsForce",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "ThicknessF",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "GuideRailss");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "GuideRailss");
        }
    }
}
