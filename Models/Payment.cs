namespace PakinProject.Models
{
    public class Payment
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // UserId ที่เชื่อมโยงกับผู้ใช้
        public string PaymentMethod { get; set; } // วิธีชำระเงิน
        public decimal Amount { get; set; } // จำนวนเงินที่จ่าย
        public DateTime PaymentDate { get; set; } // วันที่ชำระเงิน
        public string Address { get; set; } // ที่อยู่สำหรับจัดส่ง
    }

}
