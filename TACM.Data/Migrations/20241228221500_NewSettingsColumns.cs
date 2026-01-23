using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TACM.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewSettingsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NonVerbalMemoryTestDemoWordsQuantity",
                table: "settings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerbalMemoryTestDemoWordsQuantity",
                table: "settings",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NonVerbalMemoryTestDemoWordsQuantity",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "VerbalMemoryTestDemoWordsQuantity",
                table: "settings");
        }
    }
}
