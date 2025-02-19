using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_DriveSafetyBrake : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZiehlAbeggDrives_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "ZiehlAbeggDrives");

            migrationBuilder.DropColumn(
                name: "SafetyBrakeName",
                table: "ZiehlAbeggDrives");

            migrationBuilder.RenameColumn(
                name: "TypeExaminationCertificateId",
                table: "ZiehlAbeggDrives",
                newName: "DriveSafetyBrakeId");

            migrationBuilder.RenameIndex(
                name: "IX_ZiehlAbeggDrives_TypeExaminationCertificateId",
                table: "ZiehlAbeggDrives",
                newName: "IX_ZiehlAbeggDrives_DriveSafetyBrakeId");

            migrationBuilder.CreateTable(
                name: "DriveSafetyBrakes",
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
                    table.PrimaryKey("PK_DriveSafetyBrakes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriveSafetyBrakes_TypeExaminationCertificates_TypeExaminationCertificateId",
                        column: x => x.TypeExaminationCertificateId,
                        principalTable: "TypeExaminationCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriveSafetyBrakes_TypeExaminationCertificateId",
                table: "DriveSafetyBrakes",
                column: "TypeExaminationCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZiehlAbeggDrives_DriveSafetyBrakes_DriveSafetyBrakeId",
                table: "ZiehlAbeggDrives",
                column: "DriveSafetyBrakeId",
                principalTable: "DriveSafetyBrakes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZiehlAbeggDrives_DriveSafetyBrakes_DriveSafetyBrakeId",
                table: "ZiehlAbeggDrives");

            migrationBuilder.DropTable(
                name: "DriveSafetyBrakes");

            migrationBuilder.RenameColumn(
                name: "DriveSafetyBrakeId",
                table: "ZiehlAbeggDrives",
                newName: "TypeExaminationCertificateId");

            migrationBuilder.RenameIndex(
                name: "IX_ZiehlAbeggDrives_DriveSafetyBrakeId",
                table: "ZiehlAbeggDrives",
                newName: "IX_ZiehlAbeggDrives_TypeExaminationCertificateId");

            migrationBuilder.AddColumn<string>(
                name: "SafetyBrakeName",
                table: "ZiehlAbeggDrives",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ZiehlAbeggDrives_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "ZiehlAbeggDrives",
                column: "TypeExaminationCertificateId",
                principalTable: "TypeExaminationCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
