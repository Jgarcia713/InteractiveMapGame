using System.ComponentModel.DataAnnotations;

namespace InteractiveMapGame.Models
{
    public class Admin
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? FullName { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsSuperAdmin { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<AdminSession> Sessions { get; set; } = new List<AdminSession>();
    }
}
