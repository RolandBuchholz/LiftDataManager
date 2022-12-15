using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class newLoadandPersonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoadTable6s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Load = table.Column<int>(type: "INTEGER", nullable: false),
                    Area = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadTable6s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoadTable7s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Load = table.Column<int>(type: "INTEGER", nullable: false),
                    Area = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadTable7s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonsTable8s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Persons = table.Column<int>(type: "INTEGER", nullable: false),
                    Area = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonsTable8s", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoadTable6s");

            migrationBuilder.DropTable(
                name: "LoadTable7s");

            migrationBuilder.DropTable(
                name: "PersonsTable8s");
        }
    }
}
