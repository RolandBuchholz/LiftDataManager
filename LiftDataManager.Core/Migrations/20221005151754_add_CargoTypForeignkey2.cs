using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_CargoTypForeignkey2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargoTypes_CargoTypes_CargoTypeId",
                table: "CargoTypes");

            migrationBuilder.DropIndex(
                name: "IX_CargoTypes_CargoTypeId",
                table: "CargoTypes");

            migrationBuilder.DropColumn(
                name: "CargoTypeId",
                table: "CargoTypes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CargoTypeId",
                table: "CargoTypes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoTypes_CargoTypeId",
                table: "CargoTypes",
                column: "CargoTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CargoTypes_CargoTypes_CargoTypeId",
                table: "CargoTypes",
                column: "CargoTypeId",
                principalTable: "CargoTypes",
                principalColumn: "Id");
        }
    }
}
