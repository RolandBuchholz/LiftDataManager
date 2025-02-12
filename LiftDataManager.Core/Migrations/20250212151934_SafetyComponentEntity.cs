using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class SafetyComponentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ZiehlAbeggDrives",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "ZiehlAbeggDrives",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsObsolete",
                table: "ZiehlAbeggDrives",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OrderSelection",
                table: "ZiehlAbeggDrives",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SchindlerCertified",
                table: "ZiehlAbeggDrives",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ZiehlAbeggDrives");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "ZiehlAbeggDrives");

            migrationBuilder.DropColumn(
                name: "IsObsolete",
                table: "ZiehlAbeggDrives");

            migrationBuilder.DropColumn(
                name: "OrderSelection",
                table: "ZiehlAbeggDrives");

            migrationBuilder.DropColumn(
                name: "SchindlerCertified",
                table: "ZiehlAbeggDrives");
        }
    }
}
