using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LevelUp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    required_count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    total_xp = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    coin_balance = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "challenge",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reward_coins = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    reward_xp = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenge", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shop_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<int>(type: "int", nullable: false),
                    category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shop_item", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    scheduled_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    scheduled_end = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task", x => x.id);
                    table.ForeignKey(
                        name: "FK_task_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_achievement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    achievement_id = table.Column<int>(type: "int", nullable: false),
                    unlocked_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievement", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_achievement_achievement_achievement_id",
                        column: x => x.achievement_id,
                        principalTable: "achievement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_achievement_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_character",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    character_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    appearance = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_character", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_character_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    email_enabled = table.Column<bool>(type: "bit", nullable: false),
                    sms_enabled = table.Column<bool>(type: "bit", nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    time_zone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_settings_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weekly_availability",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    day_of_week = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekly_availability", x => x.id);
                    table.ForeignKey(
                        name: "FK_weekly_availability_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_challenge",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    challenge_id = table.Column<int>(type: "int", nullable: false),
                    progress = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_challenge", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_challenge_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_challenge_challenge_challenge_id",
                        column: x => x.challenge_id,
                        principalTable: "challenge",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_inventory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    shop_item_id = table.Column<int>(type: "int", nullable: false),
                    is_equipped = table.Column<bool>(type: "bit", nullable: false),
                    purchased_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_inventory", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_inventory_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_inventory_shop_item_shop_item_id",
                        column: x => x.shop_item_id,
                        principalTable: "shop_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "point_transaction",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    task_id = table.Column<int>(type: "int", nullable: true),
                    shop_item_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_point_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_point_transaction_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_point_transaction_shop_item_shop_item_id",
                        column: x => x.shop_item_id,
                        principalTable: "shop_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_point_transaction_task_task_id",
                        column: x => x.task_id,
                        principalTable: "task",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reminder",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    task_id = table.Column<int>(type: "int", nullable: true),
                    scheduled_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminder", x => x.id);
                    table.ForeignKey(
                        name: "FK_reminder_app_user_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reminder_task_task_id",
                        column: x => x.task_id,
                        principalTable: "task",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "shop_item",
                columns: new[] { "id", "category", "description", "name", "price" },
                values: new object[,]
                {
                    { 1, "Hat", "En skinnende hjelm til modige planlæggere.", "Ridderhjelm", 50 },
                    { 2, "Hat", "Spids hat med stjerner.", "Troldmandshat", 80 },
                    { 3, "Outfit", "Flagrer flot i vinden.", "Rød kappe", 60 },
                    { 4, "Outfit", "Til dem der løser opgaver i stilhed.", "Ninjadragt", 120 },
                    { 5, "Accessory", "Starterudstyr for eventyrere.", "Træsværd", 30 },
                    { 6, "Accessory", "Kun for de mest produktive.", "Gyldent skjold", 200 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_app_user_email",
                table: "app_user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_point_transaction_shop_item_id",
                table: "point_transaction",
                column: "shop_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_point_transaction_task_id",
                table: "point_transaction",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_point_transaction_user_id",
                table: "point_transaction",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminder_status_scheduled_at",
                table: "reminder",
                columns: new[] { "status", "scheduled_at" });

            migrationBuilder.CreateIndex(
                name: "IX_reminder_task_id",
                table: "reminder",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminder_user_id",
                table: "reminder",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_task_user_id_scheduled_start",
                table: "task",
                columns: new[] { "user_id", "scheduled_start" });

            migrationBuilder.CreateIndex(
                name: "IX_user_achievement_achievement_id",
                table: "user_achievement",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievement_user_id",
                table: "user_achievement",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_challenge_challenge_id",
                table: "user_challenge",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_challenge_user_id",
                table: "user_challenge",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_character_user_id",
                table: "user_character",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_inventory_shop_item_id",
                table: "user_inventory",
                column: "shop_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_inventory_user_id_shop_item_id",
                table: "user_inventory",
                columns: new[] { "user_id", "shop_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_settings_user_id",
                table: "user_settings",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weekly_availability_user_id",
                table: "weekly_availability",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "point_transaction");

            migrationBuilder.DropTable(
                name: "reminder");

            migrationBuilder.DropTable(
                name: "user_achievement");

            migrationBuilder.DropTable(
                name: "user_challenge");

            migrationBuilder.DropTable(
                name: "user_character");

            migrationBuilder.DropTable(
                name: "user_inventory");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "weekly_availability");

            migrationBuilder.DropTable(
                name: "task");

            migrationBuilder.DropTable(
                name: "achievement");

            migrationBuilder.DropTable(
                name: "challenge");

            migrationBuilder.DropTable(
                name: "shop_item");

            migrationBuilder.DropTable(
                name: "app_user");
        }
    }
}
