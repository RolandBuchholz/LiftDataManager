using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftDataManager.Core.Migrations.SafetyComponentRecord
{
    /// <inheritdoc />
    public partial class ForeignKey_SafetyComponentManfacturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SafetyComponentManfacturers",
                table: "SafetyComponentManfacturers");

            migrationBuilder.AddColumn<int>(
                name: "SafetyComponentManfacturerId",
                table: "SafetyComponentRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SafetyComponentManfacturers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "SafetyComponentManfacturers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SafetyComponentManfacturers",
                table: "SafetyComponentManfacturers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyComponentRecords_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords",
                column: "SafetyComponentManfacturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SafetyComponentRecords_SafetyComponentManfacturers_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords",
                column: "SafetyComponentManfacturerId",
                principalTable: "SafetyComponentManfacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SafetyComponentRecords_SafetyComponentManfacturers_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords");

            migrationBuilder.DropIndex(
                name: "IX_SafetyComponentRecords_SafetyComponentManfacturerId",
                table: "SafetyComponentRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SafetyComponentManfacturers",
                table: "SafetyComponentManfacturers");

            migrationBuilder.DropColumn(
                name: "SafetyComponentManfacturerId",
                table: "SafetyComponentRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "SafetyComponentManfacturers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SafetyComponentManfacturers",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SafetyComponentManfacturers",
                table: "SafetyComponentManfacturers",
                column: "Country");
        }
    }
}
