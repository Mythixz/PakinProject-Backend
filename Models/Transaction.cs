using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    public int TransactionId { get; set; } // Primary Key
    public int UserId { get; set; } // เชื่อมโยงกับผู้ใช้
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } // "Debit" หรือ "Credit"
    public string Description { get; set; }
    public DateTime TransactionDate { get; set; }
}

