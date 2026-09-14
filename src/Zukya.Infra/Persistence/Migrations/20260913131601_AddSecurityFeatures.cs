using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zukya.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(2216), new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(2218) });

            migrationBuilder.UpdateData(
                table: "system_settings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(4060), new DateTime(2026, 9, 13, 13, 7, 34, 908, DateTimeKind.Utc).AddTicks(4061) });
        }
    }
}
