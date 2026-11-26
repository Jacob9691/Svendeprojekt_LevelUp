using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LevelUp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAchievementsAndChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_challenge_user_id",
                table: "user_challenge");

            migrationBuilder.DropIndex(
                name: "IX_user_achievement_user_id",
                table: "user_achievement");

            migrationBuilder.AddColumn<Guid>(
                name: "series_id",
                table: "user_challenge",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "started_at",
                table: "user_challenge",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<string>(
                name: "repeat",
                table: "challenge",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "required_count",
                table: "challenge",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "task_title",
                table: "challenge",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "achievement",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "achievement",
                columns: new[] { "id", "description", "name", "required_count", "type" },
                values: new object[,]
                {
                    { 1, "Gennemfør din første opgave.", "Første skridt", 1, "TasksCompleted" },
                    { 2, "Gennemfør 10 opgaver.", "Godt i gang", 10, "TasksCompleted" },
                    { 3, "Gennemfør 50 opgaver.", "Opgavemester", 50, "TasksCompleted" },
                    { 4, "Nå level 5.", "Level 5", 5, "Level" },
                    { 5, "Nå level 10.", "Level 10", 10, "Level" },
                    { 6, "Nå level 20.", "Level 20", 20, "Level" },
                    { 7, "Køb din første genstand i shoppen.", "Første køb", 1, "ItemsBought" },
                    { 8, "Køb 5 genstande.", "Samler", 5, "ItemsBought" },
                    { 9, "Gennemfør din første udfordring.", "Udfordrer", 1, "ChallengesCompleted" },
                    { 10, "Gennemfør 3 udfordringer.", "Udholdende", 3, "ChallengesCompleted" }
                });

            migrationBuilder.InsertData(
                table: "challenge",
                columns: new[] { "id", "description", "end_date", "name", "repeat", "required_count", "reward_coins", "reward_xp", "task_title" },
                values: new object[,]
                {
                    { 1, "Gå en tur hver dag i en uge.", null, "7 dages gåtur", "Daily", 7, 50, 100, "Gå en tur" },
                    { 2, "Læs 20 minutter hver hverdag i to uger.", null, "Læseuge", "Weekdays", 10, 75, 150, "Læs 20 minutter" },
                    { 3, "Gør rent én gang om ugen i en måned.", null, "Rent hjem", "Weekly", 4, 60, 120, "Gør rent" },
                    { 4, "Lav din morgenrutine hver dag i 14 dage.", null, "Morgenrutine", "Daily", 14, 120, 250, "Morgenrutine" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_challenge_user_id_challenge_id",
                table: "user_challenge",
                columns: new[] { "user_id", "challenge_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_challenge_user_id_series_id",
                table: "user_challenge",
                columns: new[] { "user_id", "series_id" });

            migrationBuilder.CreateIndex(
                name: "IX_user_achievement_user_id_achievement_id",
                table: "user_achievement",
                columns: new[] { "user_id", "achievement_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_challenge_user_id_challenge_id",
                table: "user_challenge");

            migrationBuilder.DropIndex(
                name: "IX_user_challenge_user_id_series_id",
                table: "user_challenge");

            migrationBuilder.DropIndex(
                name: "IX_user_achievement_user_id_achievement_id",
                table: "user_achievement");

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "achievement",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "challenge",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "challenge",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "challenge",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "challenge",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "series_id",
                table: "user_challenge");

            migrationBuilder.DropColumn(
                name: "started_at",
                table: "user_challenge");

            migrationBuilder.DropColumn(
                name: "repeat",
                table: "challenge");

            migrationBuilder.DropColumn(
                name: "required_count",
                table: "challenge");

            migrationBuilder.DropColumn(
                name: "task_title",
                table: "challenge");

            migrationBuilder.DropColumn(
                name: "type",
                table: "achievement");

            migrationBuilder.CreateIndex(
                name: "IX_user_challenge_user_id",
                table: "user_challenge",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievement_user_id",
                table: "user_achievement",
                column: "user_id");
        }
    }
}
