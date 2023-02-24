using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class addUCMDataTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeadTime",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeadTimeSIL3",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeadTimeZAsbc4",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DetectionDistance",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DetectionDistanceSIL3",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                maxLength: 3,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Speeddetector",
                table: "LiftControlManufacturers",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SpeeddetectorSIL3",
                table: "LiftControlManufacturers",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeadTime",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "DeadTimeSIL3",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "DeadTimeZAsbc4",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "DetectionDistance",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "DetectionDistanceSIL3",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "Speeddetector",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "SpeeddetectorSIL3",
                table: "LiftControlManufacturers");
        }
    }
}
