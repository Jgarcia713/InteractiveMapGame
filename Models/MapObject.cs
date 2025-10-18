using System.ComponentModel.DataAnnotations;

namespace InteractiveMapGame.Models
{
    public class MapObject
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty; // Aircraft, Spacecraft, Museum, etc.
        
        [StringLength(100)]
        public string? Category { get; set; } // Fighter, Bomber, Satellite, etc.
        
        [StringLength(100)]
        public string? Era { get; set; } // 1960s, Modern, Future, etc.
        
        [StringLength(100)]
        public string? Manufacturer { get; set; }
        
        public DateTime? FirstFlight { get; set; }
        
        [StringLength(50)]
        public string? Status { get; set; } // Active, Retired, Prototype, etc.
        
        // Map positioning
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; } = 0; // For 3D positioning
        
        // Visual properties
        [StringLength(200)]
        public string? ImageUrl { get; set; }
        
        [StringLength(200)]
        public string? ModelUrl { get; set; } // 3D model URL
        
        [StringLength(200)]
        public string? Video360Url { get; set; } // 360 video URL
        
        // Interactive properties
        public bool IsInteractive { get; set; } = true;
        public bool IsDiscoverable { get; set; } = true;
        public bool IsUnlocked { get; set; } = false;
        
        // Game mechanics
        public int ExperienceValue { get; set; } = 0;
        public int DifficultyLevel { get; set; } = 1;
        
        // LLM-generated content
        public string? GeneratedDescription { get; set; }
        public string? GeneratedStory { get; set; }
        public string? GeneratedFacts { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
