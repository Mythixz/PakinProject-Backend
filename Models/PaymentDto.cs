using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class PaymentDto
    {
        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string SelectedPaymentMethod { get; set; }

        [Required(ErrorMessage = "กรุณากรอกที่อยู่สำหรับจัดส่ง")]
        [StringLength(255, ErrorMessage = "ที่อยู่ต้องไม่เกิน 255 ตัวอักษร")]
        public string Address { get; set; } // เพิ่มฟิลด์ Address
    }
}
