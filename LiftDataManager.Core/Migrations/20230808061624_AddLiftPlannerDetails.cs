using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddLiftPlannerDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZipCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ZipCodeNumber = table.Column<int>(type: "INTEGER", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiftPlanners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Street = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StreetNumber = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ZipCodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    MobileNumber = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftPlanners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiftPlanners_ZipCodes_ZipCodeId",
                        column: x => x.ZipCodeId,
                        principalTable: "ZipCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiftPlanners_ZipCodeId",
                table: "LiftPlanners",
                column: "ZipCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiftPlanners");

            migrationBuilder.DropTable(
                name: "ZipCodes");
        }
    }
}
