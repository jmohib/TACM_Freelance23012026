using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TACM.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    sex = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    target = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true),
                    font_size = table.Column<int>(type: "integer", nullable: true),
                    t1 = table.Column<int>(type: "integer", nullable: true),
                    t2 = table.Column<int>(type: "integer", nullable: true),
                    t3 = table.Column<int>(type: "integer", nullable: true),
                    go_probability = table.Column<int>(type: "integer", nullable: true),
                    non_probability = table.Column<int>(type: "integer", nullable: true),
                    trials = table.Column<int>(type: "integer", nullable: true),
                    rnd = table.Column<int>(type: "integer", nullable: true),
                    VerbalMemoryTestWordsQuantity = table.Column<int>(type: "integer", nullable: true),
                    NonVerbalMemoryTestWordsQuantity = table.Column<int>(type: "integer", nullable: true),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "session");

            migrationBuilder.DropTable(
                name: "settings");
        }
    }
}
