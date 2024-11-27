using Microsoft.AspNetCore.Mvc;
using PakinProject.Data;
using System.Linq;

public class WalletController : Controller
{
    private readonly PakinProjectContext _context;

    public WalletController(PakinProjectContext context)
    {
        _context = context;
    }

    // แสดงยอดเงินใน Wallet
public IActionResult Index()
{
    var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
    if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int parsedUserId))
    {
        var wallet = _context.UserWallets.FirstOrDefault(w => w.UserId == parsedUserId);
        ViewData["UserWalletBalance"] = wallet?.Balance ?? 0; // กำหนดยอดเงิน
    }
    else
    {
        ViewData["UserWalletBalance"] = 0; // กรณีผู้ใช้ไม่มี Wallet
    }

    return View();
}



    // หักเงินจาก Wallet
    [HttpPost]
    public ActionResult Deduct(decimal amount, string description)
    {
        var userId = GetLoggedInUserId();
        var wallet = _context.UserWallets.FirstOrDefault(w => w.UserId == userId);

        if (wallet == null || wallet.Balance < amount)
        {
            return Json(new { success = false, message = "Insufficient balance." });
        }

        // หักยอดเงิน
        wallet.Balance -= amount;

        // บันทึก Transaction
        var transaction = new Transaction
        {
            UserId = userId,
            Amount = amount,
            TransactionType = "Debit",
            Description = description,
            TransactionDate = DateTime.Now
        };
        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return Json(new { success = true, balance = wallet.Balance });
    }

private int GetLoggedInUserId()
{
    if (User?.Identity?.IsAuthenticated != true)
    {
        throw new UnauthorizedAccessException("No user is logged in.");
    }

    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
    if (userIdClaim == null)
    {
        throw new UnauthorizedAccessException("User ID claim is missing.");
    }

    if (!int.TryParse(userIdClaim.Value, out int userId))
    {
        throw new FormatException("User ID claim is not a valid integer.");
    }

    return userId;
}


}
