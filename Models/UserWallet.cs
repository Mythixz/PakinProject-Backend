using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserWallet
{
    [Key]
    public int WalletId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public decimal Balance { get; set; }

    // Navigation Property (เชื่อมกับ User)
    public virtual User User { get; set; }
}
