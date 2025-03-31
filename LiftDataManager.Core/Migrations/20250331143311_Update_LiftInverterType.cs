using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update_LiftInverterType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LiftInverterTypes",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "LiftInverterTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "LiftInverterTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "LiftInverterTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "LiftInverterTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TypeExaminationCertificateId",
                table: "LiftInverterTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LiftInverterTypes_TypeExaminationCertificateId",
                table: "LiftInverterTypes",
                column: "TypeExaminationCertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftInverterTypes_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "LiftInverterTypes",
                column: "TypeExaminationCertificateId",
                principalTable: "TypeExaminationCertificates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftInverterTypes_TypeExaminationCertificates_TypeExaminationCertificateId",
                table: "LiftInverterTypes");

            migrationBuilder.DropIndex(
                name: "IX_LiftInverterTypes_TypeExaminationCertificateId",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "LiftInverterTypes");

            migrationBuilder.DropColumn(
                name: "TypeExaminationCertificateId",
                table: "LiftInverterTypes");
        }
    }
}
