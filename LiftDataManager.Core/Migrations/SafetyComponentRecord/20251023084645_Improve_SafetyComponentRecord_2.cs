using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations.SafetyComponentRecord
{
    /// <inheritdoc />
    public partial class Improve_SafetyComponentRecord_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IncompleteRecord",
                table: "SafetyComponentRecords",
                newName: "CompleteRecord");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompleteRecord",
                table: "SafetyComponentRecords",
                newName: "IncompleteRecord");
        }
    }
}
