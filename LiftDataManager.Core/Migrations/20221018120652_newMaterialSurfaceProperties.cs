using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class newMaterialSurfaceProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CarMaterial",
                table: "MaterialSurfaces",
                newName: "CarMaterialSideWalls");

            migrationBuilder.AddColumn<bool>(
                name: "CarMaterialFrontBackWalls",
                table: "MaterialSurfaces",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarMaterialFrontBackWalls",
                table: "MaterialSurfaces");

            migrationBuilder.RenameColumn(
                name: "CarMaterialSideWalls",
                table: "MaterialSurfaces",
                newName: "CarMaterial");
        }
    }
}
