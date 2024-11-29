namespace PakinProject.Models
{
    public class Wallet
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // User ID สำหรับการเชื่อมโยง
        public string UserEmail { get; set; } // Email ของผู้ใช้
        public decimal Balance { get; set; } // จำนวนเงินในกระเป๋า
    }
}
