using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Transaction
{
    [Key]
    public int TransactionId { get; set; } // Primary Key

    [ForeignKey("User")]
    [Required]
    public int UserId { get; set; } // เชื่อมโยงกับผู้ใช้

    public virtual User User { get; set; } // Navigation Property

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; } // จำนวนเงิน

    [Required]
    [StringLength(50)]
    public string TransactionType { get; set; } // "Debit" หรือ "Credit"

    [StringLength(250)]
    public string Description { get; set; } // รายละเอียด

    [Required]
    public DateTime TransactionDate { get; set; } // วันที่ทำรายการ
}
