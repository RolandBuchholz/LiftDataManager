using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_SAISData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SAISManufacturerld",
                table: "TypeExaminationCertificates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "ShaftDoors",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "ShaftDoors",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "SafetyGearModelTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "SafetyGearModelTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "OverspeedGovernors",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "OverspeedGovernors",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "LiftPositionSystems",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "LiftPositionSystems",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "LiftInverterTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "LiftInverterTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "LiftDoorTelescopicAprons",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "LiftDoorTelescopicAprons",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "LiftControlManufacturers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "LiftControlManufacturers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "LiftBuffers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "LiftBuffers",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "HydraulicValves",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "HydraulicValves",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "DriveSafetyBrakes",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "DriveSafetyBrakes",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISDescription",
                table: "CarDoors",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SAISIdentificationNumber",
                table: "CarDoors",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SAISManufacturerld",
                table: "TypeExaminationCertificates");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "ShaftDoors");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "LiftDoorTelescopicAprons");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "LiftDoorTelescopicAprons");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "LiftBuffers");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "HydraulicValves");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "HydraulicValves");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "DriveSafetyBrakes");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "DriveSafetyBrakes");

            migrationBuilder.DropColumn(
                name: "SAISDescription",
                table: "CarDoors");

            migrationBuilder.DropColumn(
                name: "SAISIdentificationNumber",
                table: "CarDoors");
        }
    }
}
