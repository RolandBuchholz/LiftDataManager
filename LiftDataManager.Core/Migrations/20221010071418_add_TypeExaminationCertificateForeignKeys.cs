using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_TypeExaminationCertificateForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeExaminationCertificateId",
                table: "SafetyGearModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeExaminationCertificateId",
                table: "OverspeedGovernors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeExaminationCertificateId",
                table: "LiftPositionSystems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyGearModelTypes_TypeExaminationCertificateId",
                table: "SafetyGearModelTypes",
                column: "TypeExaminationCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_OverspeedGovernors_TypeExaminationCertificateId",
                table: "OverspeedGovernors",
                column: "TypeExaminationCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_LiftPositionSystems_TypeExaminationCertificateId",
                table: "LiftPositionSystems",
                column: "TypeExaminationCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftPositionSystems_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "LiftPositionSystems",
                column: "TypeExaminationCertificateId",
                principalTable: "TypeExaminationCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OverspeedGovernors_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "OverspeedGovernors",
                column: "TypeExaminationCertificateId",
                principalTable: "TypeExaminationCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SafetyGearModelTypes_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "SafetyGearModelTypes",
                column: "TypeExaminationCertificateId",
                principalTable: "TypeExaminationCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftPositionSystems_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "LiftPositionSystems");

            migrationBuilder.DropForeignKey(
                name: "FK_OverspeedGovernors_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "OverspeedGovernors");

            migrationBuilder.DropForeignKey(
                name: "FK_SafetyGearModelTypes_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropIndex(
                name: "IX_SafetyGearModelTypes_TypeExaminationCertificateId",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropIndex(
                name: "IX_OverspeedGovernors_TypeExaminationCertificateId",
                table: "OverspeedGovernors");

            migrationBuilder.DropIndex(
                name: "IX_LiftPositionSystems_TypeExaminationCertificateId",
                table: "LiftPositionSystems");

            migrationBuilder.DropColumn(
                name: "TypeExaminationCertificateId",
                table: "SafetyGearModelTypes");

            migrationBuilder.DropColumn(
                name: "TypeExaminationCertificateId",
                table: "OverspeedGovernors");

            migrationBuilder.DropColumn(
                name: "TypeExaminationCertificateId",
                table: "LiftPositionSystems");
        }
    }
}
