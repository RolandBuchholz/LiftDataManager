using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class FirstSetupNewSelectionParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "CENumbers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "CENumbers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "CENumbers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "CENumbers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "CENumbers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "CENumbers");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "CENumbers");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "CENumbers");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "CENumbers");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "CENumbers");
        }
    }
}
