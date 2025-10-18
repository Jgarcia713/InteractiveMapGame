using System.ComponentModel.DataAnnotations;

namespace InteractiveMapGame.Models
{
    public class PlayerProgress
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(64)]
        public string PlayerId { get; set; } = string.Empty; // Anonymous session ID
        
        [Required]
        [MaxLength(64)]
        public string SessionId { get; set; } = string.Empty;
        
        // Discovery progress
        public int DiscoveredObjects { get; set; } = 0;
        public int TotalExperience { get; set; } = 0;
        public int CurrentLevel { get; set; } = 1;
        
        // Map exploration
        public double LastX { get; set; } = 0;
        public double LastY { get; set; } = 0;
        public double LastZ { get; set; } = 0;
        
        // Game state
        public string? UnlockedObjects { get; set; } // JSON array of unlocked object IDs
        public string? CompletedQuests { get; set; } // JSON array of completed quest IDs
        public string? PlayerPreferences { get; set; } // JSON object of player settings
        
        // Statistics
        public int TotalInteractions { get; set; } = 0;
        public int VideosWatched { get; set; } = 0;
        public int TimeSpent { get; set; } = 0; // in seconds
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
    }
}
