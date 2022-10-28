using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class drop_ShaftDoorSillWidth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SillWidth",
                table: "ShaftDoors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SillWidth",
                table: "ShaftDoors",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
