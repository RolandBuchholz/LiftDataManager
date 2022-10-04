using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class CarFrameModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_ParameterDTOs",
            //    table: "ParameterDTOs");

            //migrationBuilder.RenameTable(
            //    name: "ParameterDTOs",
            //    newName: "ParameterDtos");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_ParameterDtos",
            //    table: "ParameterDtos",
            //    column: "Id");

            migrationBuilder.CreateTable(
                name: "CarFrameTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CarFrameWeight = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCFPControlled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DriveTyp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFrameTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coatings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideModelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideModelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideRailss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideRailss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideRailsStatuss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideRailsStatuss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuideTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftPositionSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftPositionSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoadWeighingDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadWeighingDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OverspeedGovernors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverspeedGovernors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReducedProtectionSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReducedProtectionSpaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyGearModelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyGearModelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyGearTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyGearTypes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarFrameTypes");

            migrationBuilder.DropTable(
                name: "Coatings");

            migrationBuilder.DropTable(
                name: "GuideModelTypes");

            migrationBuilder.DropTable(
                name: "GuideRailss");

            migrationBuilder.DropTable(
                name: "GuideRailsStatuss");

            migrationBuilder.DropTable(
                name: "GuideTypes");

            migrationBuilder.DropTable(
                name: "LiftPositionSystems");

            migrationBuilder.DropTable(
                name: "LoadWeighingDevices");

            migrationBuilder.DropTable(
                name: "OverspeedGovernors");

            migrationBuilder.DropTable(
                name: "ReducedProtectionSpaces");

            migrationBuilder.DropTable(
                name: "SafetyGearModelTypes");

            migrationBuilder.DropTable(
                name: "SafetyGearTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParameterDtos",
                table: "ParameterDtos");

            migrationBuilder.RenameTable(
                name: "ParameterDtos",
                newName: "ParameterDTOs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParameterDTOs",
                table: "ParameterDTOs",
                column: "Id");
        }
    }
}
