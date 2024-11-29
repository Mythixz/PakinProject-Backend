using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class Order
    {
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString(); // ใช้ GUID เป็น Primary Key
        
        [Required]
        public string UserEmail { get; set; } // อีเมลของผู้ใช้งาน
        
        [Required]
        public int ProductId { get; set; } // ProductId ที่เชื่อมกับ Product (เปลี่ยนเป็น int)
        
        [Required]
        public int Quantity { get; set; } // จำนวนสินค้าที่สั่ง
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; } // ยอดรวมทั้งหมด
        
        [Required]
        public string ShippingAddress { get; set; } // ที่อยู่สำหรับจัดส่งสินค้า
        
        [Required]
        public string Status { get; set; } = "Pending"; // สถานะเริ่มต้น เช่น Pending, Shipped, Completed
        
        [Required]
        public string PaymentMethod { get; set; } // วิธีการชำระเงิน เช่น Wallet, Credit Card, Cash
        
        public string Note { get; set; } // หมายเหตุสำหรับคำสั่งซื้อ
        
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now; // วันที่สั่งซื้อ
        
        public DateTime CreatedAt { get; set; } = DateTime.Now; // วันที่สร้างคำสั่งซื้อ
        
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
