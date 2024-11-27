using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLAPIBasic.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "postgres");

            migrationBuilder.CreateTable(
                name: "user_history",
                schema: "postgres",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    seq_num = table.Column<long>(type: "bigint", nullable: false),
                    login_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ip_address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_history_pkey", x => new { x.user_id, x.seq_num });
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "postgres",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    avatar_thumbnail = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_history",
                schema: "postgres");

            migrationBuilder.DropTable(
                name: "users",
                schema: "postgres");
        }
    }
}
