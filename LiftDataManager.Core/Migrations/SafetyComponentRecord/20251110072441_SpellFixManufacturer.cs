using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations.SafetyComponentRecord
{
    /// <inheritdoc />
    public partial class SpellFixManufacturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SafetyComponentRecords_SafetyComponentManfacturers_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords");

            migrationBuilder.DropTable(
                name: "SafetyComponentManfacturers");

            migrationBuilder.RenameColumn(
                name: "SafetyComponentManfacturerId",
                table: "SafetyComponentRecords",
                newName: "SafetyComponentManufacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_SafetyComponentRecords_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords",
                newName: "IX_SafetyComponentRecords_SafetyComponentManufacturerId");

            migrationBuilder.CreateTable(
                name: "SafetyComponentManufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ZIPCode = table.Column<int>(type: "INTEGER", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyComponentManufacturers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SafetyComponentRecords_SafetyComponentManufacturers_SafetyComponentManufacturerId",
                table: "SafetyComponentRecords",
                column: "SafetyComponentManufacturerId",
                principalTable: "SafetyComponentManufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SafetyComponentRecords_SafetyComponentManufacturers_SafetyComponentManufacturerId",
                table: "SafetyComponentRecords");

            migrationBuilder.DropTable(
                name: "SafetyComponentManufacturers");

            migrationBuilder.RenameColumn(
                name: "SafetyComponentManufacturerId",
                table: "SafetyComponentRecords",
                newName: "SafetyComponentManfacturerId");

            migrationBuilder.RenameIndex(
                name: "IX_SafetyComponentRecords_SafetyComponentManufacturerId",
                table: "SafetyComponentRecords",
                newName: "IX_SafetyComponentRecords_SafetyComponentManfacturerId");

            migrationBuilder.CreateTable(
                name: "SafetyComponentManfacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ZIPCode = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyComponentManfacturers", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SafetyComponentRecords_SafetyComponentManfacturers_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords",
                column: "SafetyComponentManfacturerId",
                principalTable: "SafetyComponentManfacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
