using Microsoft.AspNetCore.Mvc;
using PakinProject.Data;
using PakinProject.Models;
using System.Linq;

namespace PakinProject.Controllers
{
    public class StoreController : Controller
    {
        private readonly PakinProjectContext _context;

        public StoreController(PakinProjectContext context)
        {
            _context = context;
        }

        // แสดงสินค้าทั้งหมด, ค้นหา, และจัดเรียง
        [HttpGet]
        public IActionResult Index(string? keyword, string? sortBy, string? category)
        {
            var products = _context.Products.AsQueryable();

            // กรองสินค้าตามหมวดหมู่
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            // กรองสินค้าตามคำค้นหา (keyword)
            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(keyword) ||
                    p.Description.Contains(keyword));
            }

            // การจัดเรียงสินค้า
            switch (sortBy)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "popularity":
                    products = products.OrderByDescending(p => p.Sales);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            var productList = products.ToList();

            ViewData["CurrentKeyword"] = keyword;
            ViewData["CurrentSortBy"] = sortBy;
            ViewData["CurrentCategory"] = category; // ส่งค่าหมวดหมู่ไปยัง View

            if (!productList.Any())
            {
                ViewBag.Message = "No products found.";
            }

            return View(productList);
        }


        // แสดงรายละเอียดสินค้า
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Product ID is required.");
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id.Value);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return View(product);
        }

        // เพิ่มสินค้าลงในตะกร้า
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            if (product.StockQuantity <= 0)
            {
                TempData["Error"] = $"Product {product.ProductName} is out of stock!";
                return RedirectToAction(nameof(Index));
            }

            string customerId = User.Identity?.Name ?? "Guest";

            var cartItem = _context.CartItems.FirstOrDefault(c => c.ProductId == productId && c.CustomerID == customerId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    CustomerID = customerId
                });
            }

            product.StockQuantity--; // ลดจำนวนสินค้าในสต็อก
            _context.SaveChanges();

            TempData["Message"] = $"Product {product.ProductName} added to cart!";
            return RedirectToAction(nameof(Index));
        }

        // ชำระเงิน
        [HttpPost]
        public IActionResult Checkout()
        {
            string customerId = User.Identity?.Name ?? "Guest";

            var cartItems = _context.CartItems.Where(c => c.CustomerID == customerId).ToList();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var cartItem in cartItems)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product != null)
                {
                    // เพิ่มยอดขายของสินค้า
                    product.Sales += cartItem.Quantity;

                    // ตรวจสอบจำนวนสินค้าไม่ให้เป็นค่าลบ
                    if (product.StockQuantity < 0)
                    {
                        TempData["Error"] = $"Product {product.ProductName} has invalid stock quantity!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            try
            {
                // ลบสินค้าจากตะกร้า
                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();
                TempData["Message"] = "Payment successful! Your cart is now empty.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred during checkout: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
