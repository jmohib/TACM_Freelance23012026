using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TACM.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestResultsEndColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start",
                table: "testresultitem",
                newName: "start");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "testresultitem",
                newName: "end");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "testresult",
                newName: "end");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "start",
                table: "testresultitem",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "end",
                table: "testresultitem",
                newName: "End");

            migrationBuilder.RenameColumn(
                name: "end",
                table: "testresult",
                newName: "End");
        }
    }
}
