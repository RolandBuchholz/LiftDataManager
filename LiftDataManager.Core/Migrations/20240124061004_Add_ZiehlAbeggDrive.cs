using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_ZiehlAbeggDrive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiftBuffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeExaminationCertificateId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLoad063 = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoad063 = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLoad100 = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoad100 = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLoad130 = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoad130 = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLoad160 = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoad160 = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLoad200 = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoad200 = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLoad250 = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxLoad250 = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftBuffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiftBuffers_TypeExaminationCertificates_TypeExaminationCertificateId",
                        column: x => x.TypeExaminationCertificateId,
                        principalTable: "TypeExaminationCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VaultDocument = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZiehlAbeggDrives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeExaminationCertificateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZiehlAbeggDrives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZiehlAbeggDrives_TypeExaminationCertificates_TypeExaminationCertificateId",
                        column: x => x.TypeExaminationCertificateId,
                        principalTable: "TypeExaminationCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiftBuffers_TypeExaminationCertificateId",
                table: "LiftBuffers",
                column: "TypeExaminationCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_ZiehlAbeggDrives_TypeExaminationCertificateId",
                table: "ZiehlAbeggDrives",
                column: "TypeExaminationCertificateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiftBuffers");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "ZiehlAbeggDrives");
        }
    }
}
