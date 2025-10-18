using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasherSystem.Models
{
    public class SaledProduct
    {
        public int Id { get; set; }
        public int saledQuantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SaleId { get; set; }
        public Sale Sale { get; set; }

        // Calculated property for line total
        public decimal Total => saledQuantity * Product?.SellPrice ?? 0;
    }
}
