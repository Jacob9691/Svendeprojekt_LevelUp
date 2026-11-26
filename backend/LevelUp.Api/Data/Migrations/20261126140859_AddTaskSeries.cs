using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LevelUp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "series_id",
                table: "task",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_user_id_series_id",
                table: "task",
                columns: new[] { "user_id", "series_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_task_user_id_series_id",
                table: "task");

            migrationBuilder.DropColumn(
                name: "series_id",
                table: "task");
        }
    }
}
