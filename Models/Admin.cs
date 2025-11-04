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
        [StringLength(256)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Email { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
    }
}

