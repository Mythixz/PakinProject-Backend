using System;
using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Primary Key

        [Required]
        public string CustomerID { get; set; } // รหัสลูกค้า

        [Required]
        public string ProductCode { get; set; } // ใช้เชื่อมโยงกับ Product ผ่าน Code

        [Required]
        public int Quantity { get; set; } // จำนวนสินค้าที่สั่งซื้อ

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; } // ราคาสินค้ารวม

        [Required]
        public string ShippingAddress { get; set; } // ที่อยู่สำหรับจัดส่งสินค้า

        [Required]
        public string Status { get; set; } = "Pending"; // สถานะเริ่มต้น

        public DateTime CreatedAt { get; set; } = DateTime.Now; // วันที่สร้างคำสั่งซื้อ
    }
}
