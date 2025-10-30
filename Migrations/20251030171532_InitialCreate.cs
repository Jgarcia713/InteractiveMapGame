using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InteractiveMapGame.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InteractionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    MapObjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    InteractionType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InteractionData = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    WasSuccessful = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsedLLM = table.Column<bool>(type: "INTEGER", nullable: false),
                    LLMPrompt = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    LLMResponse = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    LLMTokens = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Era = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FirstFlight = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    X = table.Column<double>(type: "REAL", nullable: false),
                    Y = table.Column<double>(type: "REAL", nullable: false),
                    Z = table.Column<double>(type: "REAL", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ModelUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Video360Url = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsInteractive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDiscoverable = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExperienceValue = table.Column<int>(type: "INTEGER", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    GeneratedDescription = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    GeneratedStory = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    GeneratedFacts = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    DiscoveredObjects = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalExperience = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    LastX = table.Column<double>(type: "REAL", nullable: false),
                    LastY = table.Column<double>(type: "REAL", nullable: false),
                    LastZ = table.Column<double>(type: "REAL", nullable: false),
                    UnlockedObjects = table.Column<string>(type: "TEXT", nullable: true),
                    CompletedQuests = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerPreferences = table.Column<string>(type: "TEXT", nullable: true),
                    TotalInteractions = table.Column<int>(type: "INTEGER", nullable: false),
                    VideosWatched = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeSpent = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastActive = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerProgress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_MapObjectId",
                table: "InteractionLogs",
                column: "MapObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_PlayerId",
                table: "InteractionLogs",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionLogs_Timestamp",
                table: "InteractionLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_MapObjects_Category",
                table: "MapObjects",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_MapObjects_IsUnlocked",
                table: "MapObjects",
                column: "IsUnlocked");

            migrationBuilder.CreateIndex(
                name: "IX_MapObjects_Type",
                table: "MapObjects",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_MapObjects_X_Y_Z",
                table: "MapObjects",
                columns: new[] { "X", "Y", "Z" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProgress_PlayerId",
                table: "PlayerProgress",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProgress_SessionId",
                table: "PlayerProgress",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InteractionLogs");

            migrationBuilder.DropTable(
                name: "MapObjects");

            migrationBuilder.DropTable(
                name: "PlayerProgress");
        }
    }
}
