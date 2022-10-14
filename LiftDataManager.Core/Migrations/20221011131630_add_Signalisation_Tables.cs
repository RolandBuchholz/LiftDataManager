using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_Signalisation_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WeightPerSquareMeter",
                table: "CarPanels",
                newName: "Width");

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "CarPanels",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "BellPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BellPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buttons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buttons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ButtonStylusPlates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButtonStylusPlates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarCoverPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeightPerSquareMeter = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarCoverPanels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarPanelFittings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPanelFittings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarPanelMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPanelMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorDisplays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorDisplays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorLEDButtons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorLEDButtons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectionIndicatorss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectionIndicatorss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Displays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Displays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LightFieldSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightFieldSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutdoorPanelFastenings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutdoorPanelFastenings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StylusPlateMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StylusPlateMaterials", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BellPositions");

            migrationBuilder.DropTable(
                name: "Buttons");

            migrationBuilder.DropTable(
                name: "ButtonStylusPlates");

            migrationBuilder.DropTable(
                name: "CarCoverPanels");

            migrationBuilder.DropTable(
                name: "CarPanelFittings");

            migrationBuilder.DropTable(
                name: "CarPanelMaterials");

            migrationBuilder.DropTable(
                name: "ColorDisplays");

            migrationBuilder.DropTable(
                name: "ColorLEDButtons");

            migrationBuilder.DropTable(
                name: "DirectionIndicatorss");

            migrationBuilder.DropTable(
                name: "Displays");

            migrationBuilder.DropTable(
                name: "LightFieldSizes");

            migrationBuilder.DropTable(
                name: "OutdoorPanelFastenings");

            migrationBuilder.DropTable(
                name: "StylusPlateMaterials");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "CarPanels");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "CarPanels",
                newName: "WeightPerSquareMeter");
        }
    }
}
