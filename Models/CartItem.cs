using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string CustomerID { get; set; } // ID ของลูกค้า
        public int Quantity { get; set; }
        public string?  ProductCode { get; set; } // เพิ่ม property นี้
    }
}
