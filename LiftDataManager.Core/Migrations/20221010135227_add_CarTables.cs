using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_CarTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AntiDrums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntiDrums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarFloorings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Thickness = table.Column<double>(type: "REAL", nullable: false),
                    WeightPerSquareMeter = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFloorings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarFloorProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    WeightPerMeter = table.Column<double>(type: "REAL", nullable: false),
                    Height = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFloorProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarFloorSheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFloorSheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarFloorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFloorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarLightings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarLightings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FrontBackWalls = table.Column<bool>(type: "INTEGER", nullable: false),
                    SideWalls = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightPerSquareMeter = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPanels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Handrails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightPerMeter = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Handrails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftCarRoofs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftCarRoofs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftCarTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftCarTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialThicknesss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialThicknesss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mirrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    WeightPerSquareMeter = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mirrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RammingProtections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightPerMeter = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RammingProtections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkirtingBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightPerMeter = table.Column<double>(type: "REAL", nullable: false),
                    Height = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkirtingBoards", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AntiDrums");

            migrationBuilder.DropTable(
                name: "CarFloorings");

            migrationBuilder.DropTable(
                name: "CarFloorProfiles");

            migrationBuilder.DropTable(
                name: "CarFloorSheets");

            migrationBuilder.DropTable(
                name: "CarFloorTypes");

            migrationBuilder.DropTable(
                name: "CarLightings");

            migrationBuilder.DropTable(
                name: "CarMaterials");

            migrationBuilder.DropTable(
                name: "CarPanels");

            migrationBuilder.DropTable(
                name: "Handrails");

            migrationBuilder.DropTable(
                name: "LiftCarRoofs");

            migrationBuilder.DropTable(
                name: "LiftCarTypes");

            migrationBuilder.DropTable(
                name: "MaterialThicknesss");

            migrationBuilder.DropTable(
                name: "Mirrors");

            migrationBuilder.DropTable(
                name: "RammingProtections");

            migrationBuilder.DropTable(
                name: "SkirtingBoards");
        }
    }
}
