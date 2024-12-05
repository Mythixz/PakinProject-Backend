using PakinProject.Repositories;
using PakinProject.Models;

namespace PakinProject.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;

        public OrderService(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public OrderResult ProcessOrder(CheckoutDTO checkoutDTO)
        {
            // สร้างคำสั่งซื้อ
            var orderId = _orderRepository.CreateOrder(checkoutDTO);

            return new OrderResult
            {
                IsSuccess = true,
                OrderId = orderId
            };
        }
    }
}
