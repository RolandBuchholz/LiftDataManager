using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations.SafetyComponentRecord
{
    /// <inheritdoc />
    public partial class Init_SafetyComponentRecordDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiftCommissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SAISEquipment = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiftCommissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SafetyComponentRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IncompleteRecord = table.Column<bool>(type: "INTEGER", nullable: false),
                    SchindlerCertified = table.Column<bool>(type: "INTEGER", nullable: false),
                    LiftCommissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyComponentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyComponentRecords_LiftCommissions_LiftCommissionId",
                        column: x => x.LiftCommissionId,
                        principalTable: "LiftCommissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SafetyComponentRecords_LiftCommissionId",
                table: "SafetyComponentRecords",
                column: "LiftCommissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SafetyComponentRecords");

            migrationBuilder.DropTable(
                name: "LiftCommissions");
        }
    }
}
