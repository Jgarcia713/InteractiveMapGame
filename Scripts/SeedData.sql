-- Interactive Map Game - Sample Data
-- Run this script to populate the database with sample aerospace objects

-- Clear existing data (optional)
-- DELETE FROM InteractionLogs;
-- DELETE FROM PlayerProgress;
-- DELETE FROM MapObjects;

-- Insert sample map objects
INSERT INTO MapObjects (Name, Description, Type, Category, Era, Manufacturer, FirstFlight, Status, X, Y, Z, IsInteractive, IsDiscoverable, IsUnlocked, ExperienceValue, DifficultyLevel, CreatedAt, UpdatedAt)
VALUES 
-- Aircraft
('SR-71 Blackbird', 'The fastest aircraft ever built, capable of Mach 3+ speeds', 'Aircraft', 'Reconnaissance', '1960s', 'Lockheed', '1964-12-22', 'Retired', 100, 150, 0, 1, 1, 1, 100, 3, GETUTCDATE(), GETUTCDATE()),
('F-22 Raptor', 'Fifth-generation stealth fighter aircraft', 'Aircraft', 'Fighter', '2000s', 'Lockheed Martin', '1997-09-07', 'Active', 300, 250, 0, 1, 1, 0, 150, 4, GETUTCDATE(), GETUTCDATE()),
('Boeing 747', 'The original jumbo jet that revolutionized air travel', 'Aircraft', 'Commercial', '1970s', 'Boeing', '1969-02-09', 'Active', 400, 100, 0, 1, 1, 0, 80, 2, GETUTCDATE(), GETUTCDATE()),
('Concorde', 'Supersonic passenger airliner', 'Aircraft', 'Commercial', '1970s', 'Aérospatiale/BAC', '1969-03-02', 'Retired', 200, 300, 0, 1, 1, 0, 120, 3, GETUTCDATE(), GETUTCDATE()),

-- Spacecraft
('Apollo 11 Command Module', 'The spacecraft that took humans to the moon', 'Spacecraft', 'Manned', '1960s', 'North American Aviation', '1969-07-16', 'Historic', 200, 100, 0, 1, 1, 1, 200, 5, GETUTCDATE(), GETUTCDATE()),
('Space Shuttle Discovery', 'Most flown space shuttle with 39 missions', 'Spacecraft', 'Manned', '1980s', 'Rockwell International', '1984-08-30', 'Retired', 250, 300, 0, 1, 1, 0, 180, 4, GETUTCDATE(), GETUTCDATE()),
('Dragon Capsule', 'Modern commercial spacecraft for cargo and crew', 'Spacecraft', 'Manned', '2010s', 'SpaceX', '2010-12-08', 'Active', 350, 200, 0, 1, 1, 0, 160, 3, GETUTCDATE(), GETUTCDATE()),

-- Satellites
('Hubble Space Telescope', 'Revolutionary space observatory', 'Satellite', 'Scientific', '1990s', 'Lockheed Martin', '1990-04-24', 'Active', 150, 200, 0, 1, 1, 0, 140, 3, GETUTCDATE(), GETUTCDATE()),
('Sputnik 1', 'The first artificial satellite', 'Satellite', 'Scientific', '1950s', 'Soviet Union', '1957-10-04', 'Historic', 50, 50, 0, 1, 1, 1, 100, 2, GETUTCDATE(), GETUTCDATE()),
('GPS Satellite', 'Global Positioning System satellite', 'Satellite', 'Navigation', '1980s', 'Rockwell International', '1978-02-22', 'Active', 500, 400, 0, 1, 1, 0, 90, 2, GETUTCDATE(), GETUTCDATE()),

-- Rockets
('Saturn V', 'The rocket that launched Apollo missions to the moon', 'Rocket', 'Launch Vehicle', '1960s', 'Boeing/North American', '1967-11-09', 'Retired', 300, 150, 0, 1, 1, 0, 250, 5, GETUTCDATE(), GETUTCDATE()),
('Falcon Heavy', 'Most powerful operational rocket', 'Rocket', 'Launch Vehicle', '2010s', 'SpaceX', '2018-02-06', 'Active', 450, 350, 0, 1, 1, 0, 200, 4, GETUTCDATE(), GETUTCDATE()),

-- Helicopters
('Bell UH-1 Iroquois', 'Iconic Vietnam War helicopter', 'Helicopter', 'Military', '1960s', 'Bell Helicopter', '1956-10-20', 'Active', 150, 400, 0, 1, 1, 0, 70, 2, GETUTCDATE(), GETUTCDATE()),
('Apache AH-64', 'Advanced attack helicopter', 'Helicopter', 'Military', '1980s', 'Boeing', '1975-09-30', 'Active', 250, 450, 0, 1, 1, 0, 110, 3, GETUTCDATE(), GETUTCDATE()),

-- Museums and Landmarks
('National Air and Space Museum', 'Premier aerospace museum', 'Museum', 'Institution', 'Modern', 'Smithsonian', NULL, 'Active', 100, 100, 0, 1, 1, 1, 50, 1, GETUTCDATE(), GETUTCDATE()),
('Kennedy Space Center', 'NASA launch facility', 'Museum', 'Institution', 'Modern', 'NASA', NULL, 'Active', 400, 100, 0, 1, 1, 0, 80, 2, GETUTCDATE(), GETUTCDATE());

-- Insert sample player progress (optional)
INSERT INTO PlayerProgress (PlayerId, SessionId, DiscoveredObjects, TotalExperience, CurrentLevel, LastX, LastY, LastZ, UnlockedObjects, CompletedQuests, PlayerPreferences, TotalInteractions, VideosWatched, TimeSpent, CreatedAt, LastActive)
VALUES 
('player_demo_001', 'session_demo_001', 3, 150, 2, 200, 150, 0, '[1,5,9]', '[]', '{"theme":"dark","sound":true}', 12, 2, 1800, GETUTCDATE(), GETUTCDATE());

-- Insert sample interaction logs (optional)
INSERT INTO InteractionLogs (PlayerId, MapObjectId, InteractionType, InteractionData, Duration, WasSuccessful, UsedLLM, LLMPrompt, LLMResponse, LLMTokens, Timestamp)
VALUES 
('player_demo_001', 1, 'click', '{"x":100,"y":150}', 500, 1, 1, 'Generate a description for SR-71 Blackbird', 'The SR-71 Blackbird is a legendary reconnaissance aircraft...', 45, GETUTCDATE()),
('player_demo_001', 5, 'hover', '{"duration":2000}', 2000, 1, 0, NULL, NULL, NULL, GETUTCDATE()),
('player_demo_001', 9, 'click', '{"x":50,"y":50}', 300, 1, 1, 'Tell me about Sputnik 1', 'Sputnik 1 was the first artificial satellite...', 38, GETUTCDATE());

PRINT 'Sample data inserted successfully!';
PRINT 'Total MapObjects: ' + CAST((SELECT COUNT(*) FROM MapObjects) AS VARCHAR(10));
PRINT 'Total PlayerProgress: ' + CAST((SELECT COUNT(*) FROM PlayerProgress) AS VARCHAR(10));
PRINT 'Total InteractionLogs: ' + CAST((SELECT COUNT(*) FROM InteractionLogs) AS VARCHAR(10));
