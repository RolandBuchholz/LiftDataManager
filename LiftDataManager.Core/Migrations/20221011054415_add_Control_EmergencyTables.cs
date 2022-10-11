using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_Control_EmergencyTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ControlCabinetPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlCabinetPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlCabinetSizeMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlCabinetSizeMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlCabinetSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlCabinetSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriveSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsGearbox = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriveSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyCallButtons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCallButtons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyCalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyHotlines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyHotlines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftControlManufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftControlManufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerSupplys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerSupplys", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlCabinetPositions");

            migrationBuilder.DropTable(
                name: "ControlCabinetSizeMaterials");

            migrationBuilder.DropTable(
                name: "ControlCabinetSizes");

            migrationBuilder.DropTable(
                name: "ControlTypes");

            migrationBuilder.DropTable(
                name: "DriveSystems");

            migrationBuilder.DropTable(
                name: "EmergencyCallButtons");

            migrationBuilder.DropTable(
                name: "EmergencyCalls");

            migrationBuilder.DropTable(
                name: "EmergencyDevices");

            migrationBuilder.DropTable(
                name: "EmergencyHotlines");

            migrationBuilder.DropTable(
                name: "LiftControlManufacturers");

            migrationBuilder.DropTable(
                name: "PowerSupplys");
        }
    }
}
