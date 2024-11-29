namespace PakinProject.Models
{
    public class PaymentViewModel
    {
        // ข้อมูลที่อยู่ของผู้ใช้
        public string Address { get; set; }

        // รายการวิธีการชำระเงิน
        public List<string> PaymentMethods { get; set; }

        // วิธีการชำระเงินที่ผู้ใช้เลือก
        public string SelectedPaymentMethod { get; set; }

        // ข้อมูลบัตรเครดิต
        public string CreditCardNumber { get; set; }
        public string CreditCardExpiry { get; set; }
        public string CreditCardCVV { get; set; }

        // ยอดเงินในกระเป๋า
        public decimal WalletBalance { get; set; }

        // ยอดรวมที่ต้องชำระ
        public decimal TotalAmount { get; set; }
    }
}
