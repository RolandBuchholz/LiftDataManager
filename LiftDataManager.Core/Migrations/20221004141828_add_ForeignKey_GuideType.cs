using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_ForeignKey_GuideType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuideTypeId",
                table: "GuideModelTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GuideModelTypes_GuideTypeId",
                table: "GuideModelTypes",
                column: "GuideTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_GuideModelTypes_GuideTypes_GuideTypeId",
                table: "GuideModelTypes",
                column: "GuideTypeId",
                principalTable: "GuideTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuideModelTypes_GuideTypes_GuideTypeId",
                table: "GuideModelTypes");

            migrationBuilder.DropIndex(
                name: "IX_GuideModelTypes_GuideTypeId",
                table: "GuideModelTypes");

            migrationBuilder.DropColumn(
                name: "GuideTypeId",
                table: "GuideModelTypes");
        }
    }
}
