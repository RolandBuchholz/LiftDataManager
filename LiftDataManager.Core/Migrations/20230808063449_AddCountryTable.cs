using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "ZipCodes");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "ZipCodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Countrys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countrys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZipCodes_CountryId",
                table: "ZipCodes",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZipCodes_Countrys_CountryId",
                table: "ZipCodes",
                column: "CountryId",
                principalTable: "Countrys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZipCodes_Countrys_CountryId",
                table: "ZipCodes");

            migrationBuilder.DropTable(
                name: "Countrys");

            migrationBuilder.DropIndex(
                name: "IX_ZipCodes_CountryId",
                table: "ZipCodes");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "ZipCodes");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ZipCodes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
