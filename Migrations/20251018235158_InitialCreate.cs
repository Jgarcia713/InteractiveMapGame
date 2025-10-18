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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MapObjectId = table.Column<int>(type: "int", nullable: false),
                    InteractionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InteractionData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    UsedLLM = table.Column<bool>(type: "bit", nullable: false),
                    LLMPrompt = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LLMResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LLMTokens = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Era = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FirstFlight = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    X = table.Column<double>(type: "float", nullable: false),
                    Y = table.Column<double>(type: "float", nullable: false),
                    Z = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ModelUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Video360Url = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsInteractive = table.Column<bool>(type: "bit", nullable: false),
                    IsDiscoverable = table.Column<bool>(type: "bit", nullable: false),
                    IsUnlocked = table.Column<bool>(type: "bit", nullable: false),
                    ExperienceValue = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    GeneratedDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedStory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneratedFacts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DiscoveredObjects = table.Column<int>(type: "int", nullable: false),
                    TotalExperience = table.Column<int>(type: "int", nullable: false),
                    CurrentLevel = table.Column<int>(type: "int", nullable: false),
                    LastX = table.Column<double>(type: "float", nullable: false),
                    LastY = table.Column<double>(type: "float", nullable: false),
                    LastZ = table.Column<double>(type: "float", nullable: false),
                    UnlockedObjects = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedQuests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerPreferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalInteractions = table.Column<int>(type: "int", nullable: false),
                    VideosWatched = table.Column<int>(type: "int", nullable: false),
                    TimeSpent = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: false)
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
