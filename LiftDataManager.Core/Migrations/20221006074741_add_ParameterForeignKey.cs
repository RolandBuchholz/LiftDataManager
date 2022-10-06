using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_ParameterForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParameterCategory",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "ParameterTyp",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "TypeCode",
                table: "ParameterDtos");

            migrationBuilder.AddColumn<int>(
                name: "ParameterCategoryId",
                table: "ParameterDtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParameterTypId",
                table: "ParameterDtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParameterTypeCodeId",
                table: "ParameterDtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParameterDtos_ParameterCategoryId",
                table: "ParameterDtos",
                column: "ParameterCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterDtos_ParameterTypeCodeId",
                table: "ParameterDtos",
                column: "ParameterTypeCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterDtos_ParameterTypId",
                table: "ParameterDtos",
                column: "ParameterTypId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterDtos_ParameterCategorys_ParameterCategoryId",
                table: "ParameterDtos",
                column: "ParameterCategoryId",
                principalTable: "ParameterCategorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterDtos_ParameterTypeCodes_ParameterTypeCodeId",
                table: "ParameterDtos",
                column: "ParameterTypeCodeId",
                principalTable: "ParameterTypeCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterDtos_ParameterTyps_ParameterTypId",
                table: "ParameterDtos",
                column: "ParameterTypId",
                principalTable: "ParameterTyps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterDtos_ParameterCategorys_ParameterCategoryId",
                table: "ParameterDtos");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterDtos_ParameterTypeCodes_ParameterTypeCodeId",
                table: "ParameterDtos");

            migrationBuilder.DropForeignKey(
                name: "FK_ParameterDtos_ParameterTyps_ParameterTypId",
                table: "ParameterDtos");

            migrationBuilder.DropIndex(
                name: "IX_ParameterDtos_ParameterCategoryId",
                table: "ParameterDtos");

            migrationBuilder.DropIndex(
                name: "IX_ParameterDtos_ParameterTypeCodeId",
                table: "ParameterDtos");

            migrationBuilder.DropIndex(
                name: "IX_ParameterDtos_ParameterTypId",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "ParameterCategoryId",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "ParameterTypId",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "ParameterTypeCodeId",
                table: "ParameterDtos");

            migrationBuilder.AddColumn<string>(
                name: "ParameterCategory",
                table: "ParameterDtos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParameterTyp",
                table: "ParameterDtos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeCode",
                table: "ParameterDtos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
