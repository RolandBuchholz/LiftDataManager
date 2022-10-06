using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_CargoTypForeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CargoType",
                table: "LiftTypes");

            migrationBuilder.AddColumn<int>(
                name: "CargoTypeId",
                table: "LiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CargoTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CargoTypeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoTypes_CargoTypes_CargoTypeId",
                        column: x => x.CargoTypeId,
                        principalTable: "CargoTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiftTypes_CargoTypeId",
                table: "LiftTypes",
                column: "CargoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoTypes_CargoTypeId",
                table: "CargoTypes",
                column: "CargoTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftTypes_CargoTypes_CargoTypeId",
                table: "LiftTypes",
                column: "CargoTypeId",
                principalTable: "CargoTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftTypes_CargoTypes_CargoTypeId",
                table: "LiftTypes");

            migrationBuilder.DropTable(
                name: "CargoTypes");

            migrationBuilder.DropIndex(
                name: "IX_LiftTypes_CargoTypeId",
                table: "LiftTypes");

            migrationBuilder.DropColumn(
                name: "CargoTypeId",
                table: "LiftTypes");

            migrationBuilder.AddColumn<string>(
                name: "CargoType",
                table: "LiftTypes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
