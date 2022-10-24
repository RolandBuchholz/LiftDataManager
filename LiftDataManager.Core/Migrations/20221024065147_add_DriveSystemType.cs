using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class add_DriveSystemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorOpeningType",
                table: "LiftDoorGroups");

            migrationBuilder.AddColumn<int>(
                name: "LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DriveSystemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriveSystemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftDoorOpeningDirections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftDoorOpeningDirections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiftDoorGroups_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                column: "LiftDoorOpeningDirectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups",
                column: "LiftDoorOpeningDirectionId",
                principalTable: "LiftDoorOpeningDirections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftDoorGroups_LiftDoorOpeningDirections_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.DropTable(
                name: "DriveSystemTypes");

            migrationBuilder.DropTable(
                name: "LiftDoorOpeningDirections");

            migrationBuilder.DropIndex(
                name: "IX_LiftDoorGroups_LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.DropColumn(
                name: "LiftDoorOpeningDirectionId",
                table: "LiftDoorGroups");

            migrationBuilder.AddColumn<string>(
                name: "DoorOpeningType",
                table: "LiftDoorGroups",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
