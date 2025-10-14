using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasherSystem.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        public decimal Total { get; set; }
        
        public decimal Discount { get; set; } = 0;
        
        [Required]
        public decimal NetTotal { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string PaymentType { get; set; } = "cash";
        public int? paidAmount { get; set; } = 0;
        public int? remainingAmount { get; set; } = 0;

        [Required]
        [MaxLength(20)]
        public string SecretCode { get; set; } = string.Empty;
        
        public int? UserId { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserInfo? User { get; set; }
        public  ICollection<Return> Returns { get; set; } = new List<Return>();
    }
}
