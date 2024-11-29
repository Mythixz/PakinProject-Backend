using Microsoft.AspNetCore.Mvc;
using PakinProject.Data;
using PakinProject.Models;
using System;
using System.Linq;

public class PaymentController : Controller
{
    private readonly PakinProjectContext _context;

    public PaymentController(PakinProjectContext context)
    {
        _context = context;
    }

    // หน้าแสดงข้อมูลการชำระเงิน
    public IActionResult Index(decimal totalPrice)
    {
        ViewBag.TotalPrice = totalPrice;
        return View();
    }

    // ดำเนินการประมวลผลการชำระเงิน
    [HttpPost]
    public IActionResult Process(CheckoutDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        // ดึง UserId จาก Claims
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value ?? "0");
        if (userId == 0)
        {
            return Unauthorized("User not logged in.");
        }

        // ตรวจสอบวิธีการชำระเงิน
        var paymentResult = ProcessPayment(model, userId);

        if (!paymentResult.success)
        {
            ModelState.AddModelError("", paymentResult.message);
            return View("Index", model);
        }

        // บันทึกการชำระเงินในฐานข้อมูล
        SavePaymentRecord(model, userId);

        return RedirectToAction("Success");
    }

    // หน้าการชำระเงินสำเร็จ
    public IActionResult Success()
    {
        return View();
    }

    // ฟังก์ชันประมวลผลการชำระเงิน
    private (bool success, string message) ProcessPayment(CheckoutDTO model, int userId)
    {
        switch (model.PaymentMethod)
        {
            case "CreditCard":
                // จำลองการชำระเงินด้วยบัตรเครดิต (ในระบบจริงควรเชื่อมต่อ API ของ Payment Gateway)
                return (true, "Payment completed successfully.");

            case "Wallet":
                var wallet = _context.UserWallets.FirstOrDefault(w => w.UserId == userId);
                if (wallet == null || wallet.Balance < model.TotalPrice)
                {
                    return (false, "Insufficient wallet balance.");
                }

                // หักเงินจาก Wallet
                wallet.Balance -= model.TotalPrice;

                // เพิ่ม Transaction การหักเงิน
                _context.Transactions.Add(new Transaction
                {
                    UserId = userId,
                    Amount = model.TotalPrice,
                    TransactionType = "Debit",
                    Description = "Payment for order",
                    TransactionDate = DateTime.Now
                });
                return (true, "Payment completed using Wallet.");

            case "COD":
                // ไม่มีการประมวลผลเพิ่มเติมสำหรับ COD
                return (true, "Payment will be made upon delivery.");

            default:
                return (false, "Invalid payment method.");
        }
    }

    // ฟังก์ชันบันทึกการชำระเงินในฐานข้อมูล
    private void SavePaymentRecord(CheckoutDTO model, int userId)
    {
        var payment = new Payment
        {
            UserId = userId,
            PaymentMethod = model.PaymentMethod,
            Amount = model.TotalPrice,
            PaymentDate = DateTime.Now,
            Address = model.ShippingAddress
        };

        _context.Payments.Add(payment);
        _context.SaveChanges();
    }
}
