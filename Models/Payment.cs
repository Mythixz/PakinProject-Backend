using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class Payment
    {
        public int Id { get; set; } // Primary Key
        [Required]
        public string CustomerId { get; set; }
        public string PaymentMethod { get; set; } // วิธีชำระเงิน
        [Required]
        [Precision(18, 2)]
        public decimal Amount { get; set; } // จำนวนเงินที่จ่าย
        public DateTime PaymentDate { get; set; } // วันที่ชำระเงิน
        public string Address { get; set; } // ที่อยู่สำหรับจัดส่ง
    }

}
