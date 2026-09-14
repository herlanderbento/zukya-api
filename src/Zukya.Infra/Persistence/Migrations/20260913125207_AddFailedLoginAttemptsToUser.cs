using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zukya.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFailedLoginAttemptsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "failed_login_attempts",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failed_login_attempts",
                table: "users");
        }
    }
}
