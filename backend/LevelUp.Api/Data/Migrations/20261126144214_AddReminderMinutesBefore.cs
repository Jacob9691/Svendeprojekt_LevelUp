using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderMinutesBefore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reminder_minutes_before",
                table: "user_settings",
                type: "int",
                nullable: false,
                // Eksisterende brugere får samme standard som nye (30 minutter før)
                defaultValue: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reminder_minutes_before",
                table: "user_settings");
        }
    }
}
