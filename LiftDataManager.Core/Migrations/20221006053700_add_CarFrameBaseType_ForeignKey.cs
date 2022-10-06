using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_CarFrameBaseType_ForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarFrameBaseType",
                table: "CarFrameTypes");

            migrationBuilder.AddColumn<int>(
                name: "CarFrameBaseTypeId",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarFrameBaseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFrameBaseTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarFrameTypes_CarFrameBaseTypeId",
                table: "CarFrameTypes",
                column: "CarFrameBaseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarFrameTypes_CarFrameBaseTypes_CarFrameBaseTypeId",
                table: "CarFrameTypes",
                column: "CarFrameBaseTypeId",
                principalTable: "CarFrameBaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarFrameTypes_CarFrameBaseTypes_CarFrameBaseTypeId",
                table: "CarFrameTypes");

            migrationBuilder.DropTable(
                name: "CarFrameBaseTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarFrameTypes_CarFrameBaseTypeId",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "CarFrameBaseTypeId",
                table: "CarFrameTypes");

            migrationBuilder.AddColumn<string>(
                name: "CarFrameBaseType",
                table: "CarFrameTypes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
