using System.ComponentModel.DataAnnotations;

namespace InteractiveMapGame.Models
{
    public class AdminSession
    {
        public int Id { get; set; }
        
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        [StringLength(64)]
        public string SessionToken { get; set; } = string.Empty;
        
        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? UserAgent { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiresAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime? LastActivityAt { get; set; }
        
        // Navigation properties
        public virtual Admin Admin { get; set; } = null!;
    }
}
