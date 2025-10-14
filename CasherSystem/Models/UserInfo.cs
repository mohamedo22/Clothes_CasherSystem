using System.ComponentModel.DataAnnotations;

namespace CasherSystem.Models
{
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        [Phone]
        public string? PhoneNumber { get; set; }
        
        // Navigation properties
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
    }
}
