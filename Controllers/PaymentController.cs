using Microsoft.AspNetCore.Mvc;
using PakinProject.Data;
using PakinProject.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;

namespace PakinProject.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PakinProjectContext _context;

        public PaymentController(PakinProjectContext context)
        {
            _context = context;
        }

        public IActionResult Index(decimal? totalPrice)
        {
            if (totalPrice == null || totalPrice <= 0)
            {
                return RedirectToAction("Error", "Home", new { message = "Invalid total price." });
            }

            var paymentViewModel = new PaymentViewModel
            {
                TotalAmount = totalPrice.Value,
                PaymentMethods = new List<string> { "CreditCard", "COD" }
            };

            return View(paymentViewModel);
        }

        [HttpPost]
        public IActionResult Process(PaymentDto model)
        {
            if (!ModelState.IsValid)
            {
                var paymentViewModel = new PaymentViewModel
                {
                    TotalAmount = model.TotalAmount,
                    PaymentMethods = new List<string> { "CreditCard", "COD" },
                    SelectedPaymentMethod = model.SelectedPaymentMethod,
                    Address = model.Address
                };

                return View("Index", paymentViewModel);
            }

            try
            {
                var customerId = User.Identity?.Name ?? "Guest";

                var payment = new Payment
                {
                    CustomerId = customerId,
                    Amount = model.TotalAmount,
                    PaymentMethod = model.SelectedPaymentMethod,
                    PaymentDate = DateTime.Now,
                    Address = model.Address
                };

                _context.Payments.Add(payment);
                _context.SaveChanges();

                return RedirectToAction("Success", new { orderId = payment.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");

                var paymentViewModel = new PaymentViewModel
                {
                    TotalAmount = model.TotalAmount,
                    PaymentMethods = new List<string> { "CreditCard", "COD" },
                    SelectedPaymentMethod = model.SelectedPaymentMethod,
                    Address = model.Address
                };

                return View("Index", paymentViewModel);
            }
        }

        public IActionResult Success(int orderId)
        {
            var payment = _context.Payments.Find(orderId);
            if (payment == null)
            {
                return RedirectToAction("Error", "Home", new { message = "Order not found." });
            }

            var successViewModel = new PaymentSuccessViewModel
            {
                OrderId = payment.Id,
                TotalAmount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Address = payment.Address
            };

            return View(successViewModel);
        }

        public IActionResult GenerateInvoicePdf(int orderId)
        {
            var payment = _context.Payments.Find(orderId);
            if (payment == null)
            {
                return RedirectToAction("Error", "Home", new { message = "Order not found." });
            }

            var invoice = new InvoiceDTO
            {
                OrderId = orderId.ToString(),
                CustomerID = payment.CustomerId,
                ProductName = "Sample Product",
                Quantity = 1,
                TotalPrice = payment.Amount,
                ShippingAddress = payment.Address,
                CreatedAt = payment.PaymentDate
            };

            var thaiFontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Sarabun-Regular.ttf");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);

                    page.Header()
                        .Text("Invoice")
                        .FontSize(24)
                        .FontFamily(thaiFontPath)
                        .Bold()
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // คอลัมน์สำหรับคำอธิบาย
                            columns.RelativeColumn(3); // คอลัมน์สำหรับรายละเอียด
                        });

                        // ส่วนหัวของตาราง
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).Padding(5).Text("Description").FontFamily(thaiFontPath).FontSize(12).Bold();
                            header.Cell().BorderBottom(1).Padding(5).Text("Details").FontFamily(thaiFontPath).FontSize(12).Bold();
                        });

                        // เพิ่มข้อมูลลงในตาราง
                        table.Cell().BorderBottom(1).Padding(5).Text("Order ID:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.OrderId).FontFamily(thaiFontPath);

                        table.Cell().BorderBottom(1).Padding(5).Text("Customer ID:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.CustomerID).FontFamily(thaiFontPath);

                        table.Cell().BorderBottom(1).Padding(5).Text("Product Name:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.ProductName).FontFamily(thaiFontPath);

                        table.Cell().BorderBottom(1).Padding(5).Text("Quantity:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.Quantity.ToString()).FontFamily(thaiFontPath);

                        table.Cell().BorderBottom(1).Padding(5).Text("Total Price:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.TotalPrice.ToString("C", new System.Globalization.CultureInfo("th-TH"))).FontFamily(thaiFontPath);

                        table.Cell().BorderBottom(1).Padding(5).Text("Shipping Address:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.ShippingAddress).FontFamily(thaiFontPath);

                        table.Cell().BorderBottom(1).Padding(5).Text("Created At:").FontFamily(thaiFontPath).Bold();
                        table.Cell().BorderBottom(1).Padding(5).Text(invoice.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")).FontFamily(thaiFontPath);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                        .FontFamily(thaiFontPath)
                        .FontSize(10);
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return File(stream.ToArray(), "application/pdf", $"Invoice_{invoice.OrderId}.pdf");
            }
        }

        private void CellStyle(IContainer container)
        {
            container.PaddingVertical(5).Border(1).BorderColor(Colors.Grey.Lighten2);
        }
    }
}
