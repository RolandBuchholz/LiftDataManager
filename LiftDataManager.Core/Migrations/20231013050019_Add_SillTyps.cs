using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_SillTyps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "LiftDoorSills",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SillMountTyp",
                table: "LiftDoorSills",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "LiftDoorSills");

            migrationBuilder.DropColumn(
                name: "SillMountTyp",
                table: "LiftDoorSills");
        }
    }
}
