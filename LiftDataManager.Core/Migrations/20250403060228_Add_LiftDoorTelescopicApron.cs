using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_LiftDoorTelescopicApron : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiftDoorTelescopicAprons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsFavorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsObsolete = table.Column<bool>(type: "INTEGER", nullable: false),
                    SchindlerCertified = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderSelection = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeExaminationCertificateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorTelescopicAprons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiftDoorTelescopicAprons_TypeExaminationCertificates_TypeExaminationCertificateId",
                        column: x => x.TypeExaminationCertificateId,
                        principalTable: "TypeExaminationCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiftDoorTelescopicAprons_TypeExaminationCertificateId",
                table: "LiftDoorTelescopicAprons",
                column: "TypeExaminationCertificateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiftDoorTelescopicAprons");
        }
    }
}
