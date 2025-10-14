using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasherSystem.Models
{
    public class Return
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int SaleId { get; set; }
        
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        public decimal TotalRefund { get; set; }
        
        
        // Navigation properties
        [ForeignKey("SaleId")]
        public  Sale Sale { get; set; } = null!;
        public List<Product> products { get; set; } = new List<Product>();
    }
}
