using PakinProject.Models;
using PakinProject.Repositories;

namespace PakinProject.Services
{
    public class OrderService
    {
        private readonly WalletRepository _walletRepository;
        private readonly OrderRepository _orderRepository;

        public OrderService(WalletRepository walletRepository, OrderRepository orderRepository)
        {
            _walletRepository = walletRepository;
            _orderRepository = orderRepository;
        }

        public OrderResult ProcessOrder(CheckoutDTO checkoutDTO)
        {
            // ดึง UserId จาก UserEmail
            var userId = _walletRepository.GetUserIdByEmail(checkoutDTO.UserEmail);
            if (userId == 0) // ตรวจสอบว่าพบ UserId หรือไม่
            {
                return new OrderResult
                {
                    IsSuccess = false,
                    ErrorMessage = "User not found."
                };
            }

            // ตรวจสอบยอดเงินใน Wallet
            decimal walletBalance = _walletRepository.GetWalletBalance(checkoutDTO.UserEmail);
            if (walletBalance < checkoutDTO.TotalPrice) // ตรวจสอบว่ายอดเงินเพียงพอหรือไม่
            {
                return new OrderResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Insufficient balance in wallet."
                };
            }

            // สร้างคำสั่งซื้อ
            var orderId = _orderRepository.CreateOrder(checkoutDTO);

            // หักเงินใน Wallet
            _walletRepository.DeductBalance(userId, checkoutDTO.TotalPrice);

            // ส่งผลลัพธ์สำเร็จ
            return new OrderResult
            {
                IsSuccess = true,
                ErrorMessage = null
            };
        }
    }
}
