using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PakinProject.Data;
using System.Linq;

namespace PakinProject.Filters;
public class WalletBalanceFilter : IActionFilter
{
    private readonly PakinProjectContext _context;

    public WalletBalanceFilter(PakinProjectContext context)
    {
        _context = context;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Controller;
        if (controller != null && controller.User.Identity.IsAuthenticated)
        {
            // ดึง UserId จาก Claims
            var userIdClaim = controller.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                // ดึงข้อมูล Wallet จากฐานข้อมูล
                var wallet = _context.UserWallets.FirstOrDefault(w => w.UserId == userId);
                controller.ViewData["UserWalletBalance"] = wallet?.Balance ?? 0; // ตั้งค่า Balance ใน ViewData
            }
            else
            {
                controller.ViewData["UserWalletBalance"] = 0; // หากไม่มี Wallet
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // ไม่ต้องทำอะไรหลังจาก Action ถูกเรียก
    }
}
