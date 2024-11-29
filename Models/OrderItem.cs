using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PakinProject.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        public string OrderId { get; set; } // ใช้ string แทน int

        [Required]
        public int ProductId { get; set; } // เชื่อมโยงกับ Product

        [Required]
        public int Quantity { get; set; } // จำนวนสินค้าที่สั่งซื้อ

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } // ราคาสินค้า

        // เพิ่ม Navigation Property เชื่อมกลับไปที่ Order
        public virtual Order Order { get; set; }
    }
}
