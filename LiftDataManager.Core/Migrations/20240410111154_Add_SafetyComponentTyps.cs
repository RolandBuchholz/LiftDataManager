using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_SafetyComponentTyps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SafetyComponentTypId",
                table: "TypeExaminationCertificates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SafetyComponentTyps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyComponentTyps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeExaminationCertificates_SafetyComponentTypId",
                table: "TypeExaminationCertificates",
                column: "SafetyComponentTypId");

            migrationBuilder.AddForeignKey(
                name: "FK_TypeExaminationCertificates_SafetyComponentTyps_SafetyComponentTypId",
                table: "TypeExaminationCertificates",
                column: "SafetyComponentTypId",
                principalTable: "SafetyComponentTyps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypeExaminationCertificates_SafetyComponentTyps_SafetyComponentTypId",
                table: "TypeExaminationCertificates");

            migrationBuilder.DropTable(
                name: "SafetyComponentTyps");

            migrationBuilder.DropIndex(
                name: "IX_TypeExaminationCertificates_SafetyComponentTypId",
                table: "TypeExaminationCertificates");

            migrationBuilder.DropColumn(
                name: "SafetyComponentTypId",
                table: "TypeExaminationCertificates");
        }
    }
}
