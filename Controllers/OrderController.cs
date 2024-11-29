using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PakinProject.Data;
using PakinProject.Models;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PakinProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly PakinProjectContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(PakinProjectContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

            string quantityDisplay;
            if (int.TryParse(order.Quantity.ToString(), out int quantityInt))
            {
                quantityDisplay = quantityInt.ToString();
            }
            else
            {
                quantityDisplay = "Invalid quantity";
            }

            // สร้าง PDF
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);

            gfx.DrawString($"Invoice for Order {orderId}", font, XBrushes.Black, 20, 40);
            gfx.DrawString($"Customer ID: {order.UserEmail}", font, XBrushes.Black, 20, 60);
            gfx.DrawString($"Product: {product.ProductName}", font, XBrushes.Black, 20, 80);
            gfx.DrawString($"Quantity: {quantityDisplay}", font, XBrushes.Black, 20, 100);
            gfx.DrawString($"Total Price: {order.TotalPrice:C}", font, XBrushes.Black, 20, 120);
            gfx.DrawString($"Shipping Address: {order.ShippingAddress}", font, XBrushes.Black, 20, 140);
            gfx.DrawString($"Order Date: {order.CreatedAt:yyyy-MM-dd}", font, XBrushes.Black, 20, 160);

            using (var ms = new MemoryStream())
            {
                document.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return File(ms.ToArray(), "application/pdf", $"Invoice_Order_{orderId}.pdf");
            }
        }

        // ฟังก์ชันสร้างคำสั่งซื้อใหม่ - GET
        [HttpGet]
        public IActionResult Create()
        {
            var userEmail = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.CustomerEmail = userEmail;
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        // ฟังก์ชันสร้างคำสั่งซื้อใหม่ - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("ProductId", "Product not found.");
                    return View(order);
                }

                var email = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(email))
                {
                    order.UserEmail = email;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to determine logged-in user.");
                    return View(order);
                }

                if (int.TryParse(order.Quantity.ToString(), out int quantityInt))
                {
                    order.TotalPrice = product.Price * quantityInt;
                }
                else
                {
                    ModelState.AddModelError("Quantity", "Invalid quantity format. Please enter a number.");
                    return View(order);
                }

                order.OrderId = Guid.NewGuid().ToString();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var invoiceDTO = new InvoiceDTO
                {
                    OrderId = order.OrderId,
                    CustomerID = order.UserEmail,
                    ProductName = product.ProductName,
                    Quantity = quantityInt,
                    TotalPrice = order.TotalPrice,
                    ShippingAddress = order.ShippingAddress,
                    CreatedAt = order.CreatedAt
                };

                return View("Invoice", invoiceDTO);
            }

            ViewBag.Products = _context.Products.ToList();
            return View(order);
        }
    }
}
