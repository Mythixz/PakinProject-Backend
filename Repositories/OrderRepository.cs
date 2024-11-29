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
            var order = new Order
            {
                UserEmail = checkoutDTO.UserEmail,
                TotalPrice = checkoutDTO.TotalPrice,  // เปลี่ยนจาก TotalAmount เป็น TotalPrice
                OrderDate = DateTime.Now,
                ShippingAddress = checkoutDTO.ShippingAddress,
                PaymentMethod = checkoutDTO.PaymentMethod,  // เพิ่มการจัดการ PaymentMethod
                Note = checkoutDTO.Note                     // เพิ่มการจัดการ Note
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // เพิ่มรายการสินค้าใน Order
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

            return order.OrderId; // ส่งกลับ OrderId
        }
    }
}
