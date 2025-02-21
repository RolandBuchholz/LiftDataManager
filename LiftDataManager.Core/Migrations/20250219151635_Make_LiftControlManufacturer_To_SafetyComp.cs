using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Make_LiftControlManufacturer_To_SafetyComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeExaminationCertificateId",
                table: "LiftControlManufacturers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LiftControlManufacturers_TypeExaminationCertificateId",
                table: "LiftControlManufacturers",
                column: "TypeExaminationCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftControlManufacturers_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "LiftControlManufacturers",
                column: "TypeExaminationCertificateId",
                principalTable: "TypeExaminationCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftControlManufacturers_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "LiftControlManufacturers");

            migrationBuilder.DropIndex(
                name: "IX_LiftControlManufacturers_TypeExaminationCertificateId",
                table: "LiftControlManufacturers");

            migrationBuilder.DropColumn(
                name: "TypeExaminationCertificateId",
                table: "LiftControlManufacturers");
        }
    }
}
