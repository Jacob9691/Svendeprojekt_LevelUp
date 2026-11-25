using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_app_user_email",
                table: "app_user");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "app_user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "app_user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "access_failed_count",
                table: "app_user",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "concurrency_stamp",
                table: "app_user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "email_confirmed",
                table: "app_user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "lockout_enabled",
                table: "app_user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lockout_end",
                table: "app_user",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                table: "app_user",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_user_name",
                table: "app_user",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "app_user",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "phone_number_confirmed",
                table: "app_user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "security_stamp",
                table: "app_user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "two_factor_enabled",
                table: "app_user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "two_factor_method",
                table: "app_user",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Email");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "app_user",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            // Telefonnummeret flyttes fra user_settings til app_user, før kolonnen slettes
            migrationBuilder.Sql("""
                UPDATE u SET u.phone_number = s.phone_number
                FROM app_user u
                JOIN user_settings s ON s.user_id = u.id
                WHERE s.phone_number IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "user_settings");

            // Eksisterende brugere får de felter Identity kræver for at kunne logge ind
            migrationBuilder.Sql("""
                UPDATE app_user SET
                    user_name = email,
                    normalized_user_name = UPPER(email),
                    normalized_email = UPPER(email),
                    security_stamp = CONVERT(nvarchar(36), NEWID()),
                    concurrency_stamp = CONVERT(nvarchar(36), NEWID()),
                    email_confirmed = 1,
                    lockout_enabled = 1,
                    two_factor_enabled = 1;
                """);

            migrationBuilder.CreateTable(
                name: "user_claim",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    claim_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claim_value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_claim_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    provider_key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    provider_display_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_user_login_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    login_provider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_user_token_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "app_user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_email",
                table: "app_user",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "app_user",
                column: "normalized_user_name",
                unique: true,
                filter: "[normalized_user_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_claim_user_id",
                table: "user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_login_user_id",
                table: "user_login",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_claim");

            migrationBuilder.DropTable(
                name: "user_login");

            migrationBuilder.DropTable(
                name: "user_token");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "app_user");

            migrationBuilder.DropIndex(
                name: "IX_app_user_email",
                table: "app_user");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "access_failed_count",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "concurrency_stamp",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "email_confirmed",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "lockout_enabled",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "lockout_end",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "normalized_email",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "normalized_user_name",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "phone_number_confirmed",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "security_stamp",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "two_factor_enabled",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "two_factor_method",
                table: "app_user");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "app_user");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "user_settings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "app_user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "app_user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_app_user_email",
                table: "app_user",
                column: "email",
                unique: true);
        }
    }
}
