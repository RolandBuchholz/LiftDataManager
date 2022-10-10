using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class remove_properties_TypeExaminationCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "TypeExaminationCertificates");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "TypeExaminationCertificates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "TypeExaminationCertificates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "TypeExaminationCertificates",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }
    }
}
