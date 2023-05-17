using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class addsafetyGearLoads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowableWidth",
                table: "SafetyGearModelTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoadDryColddrawn",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoadDryMachined",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoadOiledColddrawn",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxLoadOiledMachined",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinLoadDryColddrawn",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinLoadDryMachined",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinLoadOiledColddrawn",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinLoadOiledMachined",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowableWidth",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MaxLoadDryColddrawn",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MaxLoadDryMachined",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MaxLoadOiledColddrawn",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MaxLoadOiledMachined",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MinLoadDryColddrawn",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MinLoadDryMachined",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MinLoadOiledColddrawn",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "MinLoadOiledMachined",
                table: "SafetyGearModelTypes");
        }
    }
}
