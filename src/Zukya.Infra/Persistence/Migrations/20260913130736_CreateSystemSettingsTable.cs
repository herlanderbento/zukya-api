using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zukya.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateSystemSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_settings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "Id", "created_at", "description", "key", "type", "updated_at", "value" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(2216), "Habilita/Desabilita banimento automatico por tentativas excessivas de login", "security.auto_banned_on_failed_login.enabled", "boolean", new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(2218), "true" },
                    { 2, new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(4060), "Numero maximo de tentativas falhadas antes de banir a conta", "security.max_failed_login_attempts", "int", new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(4061), "5" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_settings");
        }
    }
}
