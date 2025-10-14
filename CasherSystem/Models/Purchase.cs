using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasherSystem.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        [MaxLength(100)]
        public Supplier Supplier { get; set; }
        
        [Required]
        public decimal Total { get; set; }
        
        [Required]
        public decimal PaidAmount { get; set; }
        
        [Required]
        public decimal RemainingAmount { get; set; }

    }
}
