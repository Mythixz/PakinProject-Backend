using Microsoft.AspNetCore.Mvc;
using PakinProject.Models;
using PakinProject.Services;

namespace PakinProject.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly OrderService _orderService;

        public CheckoutController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult ProcessCheckout([FromBody] CheckoutDTO checkoutDTO)
        {
            var result = _orderService.ProcessOrder(checkoutDTO);

            if (result.IsSuccess)
            {
                return Json(new { success = true, orderId = result.OrderId });
            }
            else
            {
                return Json(new { success = false, message = result.ErrorMessage });
            }
        }
    }
}
