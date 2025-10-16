using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace CasherSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Barcode { get; set; }
        
        [MaxLength(20)]
        public string? Size { get; set; }
        
        [MaxLength(30)]
        public string? Color { get; set; }
        
        [Required]
        public decimal CostPrice { get; set; }
        
        [Required]
        public decimal SellPrice { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        public int counterOfSell { get; set; } = 0;

        [Required]
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<SaledProduct> Sales { get; set; } = new List<SaledProduct>();
        public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
    }
}
