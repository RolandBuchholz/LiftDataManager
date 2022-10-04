using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    public partial class addMoreProbertiesToCarFrameTyp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarFrameBaseType",
                table: "CarFrameTypes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasMachineRoom",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarFrameBaseType",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "HasMachineRoom",
                table: "CarFrameTypes");
        }
    }
}
