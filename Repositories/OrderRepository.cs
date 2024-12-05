using PakinProject.Data;
using PakinProject.Models;
using System;
using System.Linq;

namespace PakinProject.Repositories
{
    public class OrderRepository
    {
        private readonly PakinProjectContext _context;

        public OrderRepository(PakinProjectContext context)
        {
            _context = context;
        }

        // ฟังก์ชันสร้างคำสั่งซื้อ
        public string CreateOrder(CheckoutDTO checkoutDTO)
        {
            // ตรวจสอบว่ารายการสินค้าไม่ว่าง
            if (checkoutDTO.CartItems == null || !checkoutDTO.CartItems.Any())
            {
                throw new InvalidOperationException("Cart is empty. Cannot create an order.");
            }

            // ตรวจสอบความถูกต้องของราคาสินค้าและจำนวน
            foreach (var item in checkoutDTO.CartItems)
            {
                if (item.Quantity <= 0 || item.Price <= 0)
                {
                    throw new InvalidOperationException("Invalid product quantity or price.");
                }
            }

            using var transaction = _context.Database.BeginTransaction(); // ใช้ Transaction เพื่อความปลอดภัย
            try
            {
                // สร้างคำสั่งซื้อ
                var order = new Order
                {
                    UserEmail = checkoutDTO.UserEmail,
                    TotalPrice = checkoutDTO.TotalPrice, // เปลี่ยนจาก TotalAmount เป็น TotalPrice
                    OrderDate = DateTime.Now,
                    ShippingAddress = checkoutDTO.ShippingAddress,
                    PaymentMethod = checkoutDTO.PaymentMethod, // เพิ่มการจัดการ PaymentMethod
                    Note = checkoutDTO.Note // เพิ่มการจัดการ Note
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // เพิ่มรายการสินค้าในคำสั่งซื้อ
                foreach (var item in checkoutDTO.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId, // ใช้ OrderId แทน Id
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                _context.SaveChanges();

                transaction.Commit(); // Commit Transaction
                return order.OrderId; // ส่งกลับ OrderId
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // Rollback หากเกิดข้อผิดพลาด
                throw new InvalidOperationException($"Failed to create order: {ex.Message}");
            }
        }
    }
}
