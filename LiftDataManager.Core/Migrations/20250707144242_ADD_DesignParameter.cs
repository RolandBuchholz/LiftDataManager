using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class ADD_DesignParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CarDesignRelated",
                table: "ParameterDtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DispoPlanRelated",
                table: "ParameterDtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LiftPanelRelated",
                table: "ParameterDtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarDesignRelated",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "DispoPlanRelated",
                table: "ParameterDtos");

            migrationBuilder.DropColumn(
                name: "LiftPanelRelated",
                table: "ParameterDtos");
        }
    }
}
