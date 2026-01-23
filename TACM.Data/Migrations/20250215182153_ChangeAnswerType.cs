using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TACM.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAnswerType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<char>(
                name: "answer",
                table: "testresultitem",
                type: "character(1)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "answer",
                table: "testresultitem",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(char),
                oldType: "character(1)");
        }
    }
}
