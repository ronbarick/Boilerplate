using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migration.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiProvider2FA : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthenticatorKey",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthenticatorEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "TwoFactorCodes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticatorKey",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsAuthenticatorEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "TwoFactorCodes");
        }
    }
}
