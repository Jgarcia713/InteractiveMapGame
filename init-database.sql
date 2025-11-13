-- SQL script to create the Admins table if it doesn't exist

-- Create Admins table
CREATE TABLE IF NOT EXISTS "Admins" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Admins" PRIMARY KEY AUTOINCREMENT,
    "Username" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Email" TEXT,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "LastLoginAt" TEXT
);

-- Create unique index on Username
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Admins_Username" ON "Admins" ("Username");

-- Create MapObjects table if it doesn't exist
CREATE TABLE IF NOT EXISTS "MapObjects" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MapObjects" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Type" TEXT NOT NULL,
    "Description" TEXT,
    "Category" TEXT,
    "Era" TEXT,
    "Manufacturer" TEXT,
    "FirstFlight" TEXT,
    "Status" TEXT,
    "X" REAL NOT NULL,
    "Y" REAL NOT NULL,
    "Z" REAL NOT NULL,
    "ImageUrl" TEXT,
    "ModelUrl" TEXT,
    "Video360Url" TEXT,
    "IsInteractive" INTEGER NOT NULL,
    "IsDiscoverable" INTEGER NOT NULL,
    "IsUnlocked" INTEGER NOT NULL,
    "ExperienceValue" INTEGER NOT NULL,
    "DifficultyLevel" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);

-- Create InteractionLogs table if it doesn't exist
CREATE TABLE IF NOT EXISTS "InteractionLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_InteractionLogs" PRIMARY KEY AUTOINCREMENT,
    "PlayerId" TEXT NOT NULL,
    "MapObjectId" INTEGER NOT NULL,
    "InteractionType" TEXT NOT NULL,
    "InteractionData" TEXT,
    "Duration" INTEGER NOT NULL,
    "WasSuccessful" INTEGER NOT NULL,
    "UsedLLM" INTEGER NOT NULL,
    "LLMPrompt" TEXT,
    "LLMResponse" TEXT,
    "LLMTokens" INTEGER,
    "Timestamp" TEXT NOT NULL
);


