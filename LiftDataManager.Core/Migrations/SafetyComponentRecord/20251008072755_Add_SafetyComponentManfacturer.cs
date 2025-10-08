using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations.SafetyComponentRecord
{
    /// <inheritdoc />
    public partial class Add_SafetyComponentManfacturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "SafetyComponentRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentificationNumber",
                table: "SafetyComponentRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Release",
                table: "SafetyComponentRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Revision",
                table: "SafetyComponentRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "SafetyComponentRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "LiftCommissions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "LiftCommissions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HouseNumber",
                table: "LiftCommissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LiftInstallerID",
                table: "LiftCommissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "LiftCommissions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZIPCode",
                table: "LiftCommissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SafetyComponentManfacturers",
                columns: table => new
                {
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    Street = table.Column<string>(type: "TEXT", nullable: true),
                    HouseNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ZIPCode = table.Column<int>(type: "INTEGER", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyComponentManfacturers", x => x.Country);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SafetyComponentManfacturers");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "SafetyComponentRecords");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "SafetyComponentRecords");

            migrationBuilder.DropColumn(
                name: "Release",
                table: "SafetyComponentRecords");

            migrationBuilder.DropColumn(
                name: "Revision",
                table: "SafetyComponentRecords");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "SafetyComponentRecords");

            migrationBuilder.DropColumn(
                name: "City",
                table: "LiftCommissions");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "LiftCommissions");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "LiftCommissions");

            migrationBuilder.DropColumn(
                name: "LiftInstallerID",
                table: "LiftCommissions");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "LiftCommissions");

            migrationBuilder.DropColumn(
                name: "ZIPCode",
                table: "LiftCommissions");
        }
    }
}
