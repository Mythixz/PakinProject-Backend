using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PakinProject.Data;
using PakinProject.Models;
using PakinProject.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PakinProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService; // Inject OrderService
        private readonly PakinProjectContext _context;

        public OrderController(OrderService orderService, PakinProjectContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        // ฟังก์ชันสร้างใบแจ้งหนี้ในรูปแบบ PDF
        public async Task<IActionResult> GenerateInvoice(string orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.ProductId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // สร้าง PDF ด้วย QuestPDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text($"Invoice for Order {orderId}").Bold().FontSize(20).AlignCenter();

                    page.Content().PaddingVertical(10).Column(column =>
                    {
                        column.Item().Text($"Customer ID: {order.UserEmail}").Bold();
                        column.Item().Text($"Product: {product.ProductName}");
                        column.Item().Text($"Quantity: {order.Quantity}");
                        column.Item().Text($"Total Price: {order.TotalPrice.ToString("C", new System.Globalization.CultureInfo("th-TH"))}");
                        column.Item().Text($"Shipping Address: {order.ShippingAddress}");
                        column.Item().Text($"Order Date: {order.CreatedAt:yyyy-MM-dd}");
                    });

                    page.Footer().AlignRight().Text("Thank you for your purchase!");
                });
            });

            // แปลง PDF เป็นไฟล์และส่งออก
            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", $"Invoice_Order_{orderId}.pdf");
            }
        }

        // ฟังก์ชันสร้างคำสั่งซื้อใหม่ - GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        // ฟังก์ชันสร้างคำสั่งซื้อใหม่ - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CheckoutDTO checkoutDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _context.Products.ToList();
                return View(checkoutDTO);
            }

            // ใช้งาน OrderService เพื่อประมวลผลคำสั่งซื้อ
            var result = _orderService.ProcessOrder(checkoutDTO);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                ViewBag.Products = _context.Products.ToList();
                return View(checkoutDTO);
            }

            return RedirectToAction("GenerateInvoice", new { orderId = result.OrderId });
        }
    }
}
