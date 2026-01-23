using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TACM.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestResultsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_delete",
                table: "settings",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "is_delete",
                table: "session",
                newName: "is_deleted");

            migrationBuilder.CreateTable(
                name: "testresult",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    items_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    preview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testresult", x => x.id);
                    table.ForeignKey(
                        name: "FK_testresult_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testresultitem",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_result_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item1 = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    item2 = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    answer = table.Column<byte>(type: "smallint", nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testresultitem", x => x.id);
                    table.ForeignKey(
                        name: "FK_testresultitem_session_session_id",
                        column: x => x.session_id,
                        principalTable: "session",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_testresultitem_testresult_test_result_id",
                        column: x => x.test_result_id,
                        principalTable: "testresult",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_testresult_session_id",
                table: "testresult",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_testresultitem_session_id",
                table: "testresultitem",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_testresultitem_test_result_id",
                table: "testresultitem",
                column: "test_result_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "testresultitem");

            migrationBuilder.DropTable(
                name: "testresult");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "settings",
                newName: "is_delete");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "session",
                newName: "is_delete");
        }
    }
}
