using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class SchachtModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FireClosureBys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireClosureBys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FireClosures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireClosures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineRoomPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineRoomPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RailBracketFixings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RailBracketFixings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShaftFrameFieldFillings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShaftFrameFieldFillings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShaftTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShaftTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmokeExtractionShafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmokeExtractionShafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WallMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WallMaterials", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FireClosureBys");

            migrationBuilder.DropTable(
                name: "FireClosures");

            migrationBuilder.DropTable(
                name: "MachineRoomPositions");

            migrationBuilder.DropTable(
                name: "RailBracketFixings");

            migrationBuilder.DropTable(
                name: "ShaftFrameFieldFillings");

            migrationBuilder.DropTable(
                name: "ShaftTypes");

            migrationBuilder.DropTable(
                name: "SmokeExtractionShafts");

            migrationBuilder.DropTable(
                name: "WallMaterials");
        }
    }
}
