using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_DoorTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarDoors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    SillWidth = table.Column<double>(type: "REAL", nullable: false),
                    MinimalMountingSpace = table.Column<double>(type: "REAL", nullable: false),
                    DoorPanelWidth = table.Column<double>(type: "REAL", nullable: false),
                    DoorPanelSpace = table.Column<double>(type: "REAL", nullable: false),
                    DoorPanelCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeExaminationCertificateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDoors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarDoors_TypeExaminationCertificates_TypeExaminationCertificateId",
                        column: x => x.TypeExaminationCertificateId,
                        principalTable: "TypeExaminationCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorControls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorControls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DoorPanelCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorGuards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorGuards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorLockingDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorLockingDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorMaterials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorSills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorSills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorStandards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorStandards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShaftDoors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    SillWidth = table.Column<double>(type: "REAL", nullable: false),
                    DoorPanelCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeExaminationCertificateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShaftDoors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShaftDoors_TypeExaminationCertificates_TypeExaminationCertificateId",
                        column: x => x.TypeExaminationCertificateId,
                        principalTable: "TypeExaminationCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarDoors_TypeExaminationCertificateId",
                table: "CarDoors",
                column: "TypeExaminationCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_ShaftDoors_TypeExaminationCertificateId",
                table: "ShaftDoors",
                column: "TypeExaminationCertificateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarDoors");

            migrationBuilder.DropTable(
                name: "LiftDoorControls");

            migrationBuilder.DropTable(
                name: "LiftDoorGroups");

            migrationBuilder.DropTable(
                name: "LiftDoorGuards");

            migrationBuilder.DropTable(
                name: "LiftDoorLockingDevices");

            migrationBuilder.DropTable(
                name: "LiftDoorMaterials");

            migrationBuilder.DropTable(
                name: "LiftDoorSills");

            migrationBuilder.DropTable(
                name: "LiftDoorStandards");

            migrationBuilder.DropTable(
                name: "LiftDoorTypes");

            migrationBuilder.DropTable(
                name: "ShaftDoors");
        }
    }
}
