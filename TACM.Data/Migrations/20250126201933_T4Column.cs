using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TACM.Data.Migrations
{
    /// <inheritdoc />
    public partial class T4Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "T4",
                table: "settings",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "T4",
                table: "settings");
        }
    }
}
