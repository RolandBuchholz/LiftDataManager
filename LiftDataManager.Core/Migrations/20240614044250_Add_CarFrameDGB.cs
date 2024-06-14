using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class Add_CarFrameDGB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CarFrameDGB",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CarFrameDGBOffset",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CounterweightDGB",
                table: "CarFrameTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarFrameDGB",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "CarFrameDGBOffset",
                table: "CarFrameTypes");

            migrationBuilder.DropColumn(
                name: "CounterweightDGB",
                table: "CarFrameTypes");
        }
    }
}
