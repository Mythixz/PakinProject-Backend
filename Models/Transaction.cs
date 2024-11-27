using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    [Key]
    public int TransactionId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    public decimal Amount { get; set; }
    public string TransactionType { get; set; } // "Debit" หรือ "Credit"
    public string Description { get; set; }
    public DateTime TransactionDate { get; set; }

    // Navigation Property (เชื่อมกับ User)
    public virtual User User { get; set; }
}
