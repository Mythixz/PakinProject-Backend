using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PakinProject.Models; // แทนที่ด้วย namespace ของโปรเจคจริงของคุณ
using PakinProject.Data; // ใช้สำหรับเข้าถึงฐานข้อมูล
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ShoppingCartController : Controller
{
    private readonly PakinProjectContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShoppingCartController(PakinProjectContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    // แสดงตะกร้าสินค้า
    public IActionResult Index()
    {
        var userEmail = _httpContextAccessor.HttpContext.User.Identity.Name;

        var cartItems = (from cart in _context.CartItems
                         join product in _context.Products on cart.ProductId equals product.Id
                         where cart.CustomerID == userEmail
                         select new CartItemDto
                         {
                             CartItemId = cart.CartItemId,
                             ProductId = product.Id,
                             ProductName = product.ProductName,
                             Price = product.Price,
                             Quantity = cart.Quantity,
                             Total = cart.Quantity * product.Price // คำนวณราคาทั้งหมด
                         }).ToList();

        // คำนวณราคารวม
        ViewBag.TotalPrice = cartItems.Sum(item => item.Total);
        return View(cartItems);
    }

    // เพิ่มสินค้าลงในตะกร้า
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        var userEmail = _httpContextAccessor.HttpContext.User.Identity.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "Account");
        }

        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.StockQuantity <= 0)
        {
            TempData["ErrorMessage"] = "สินค้าหมดสต็อกหรือจำนวนไม่พอ";
            return RedirectToAction("Index");
        }

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.ProductId == productId && c.CustomerID == userEmail);

        if (cartItem != null)
        {
            cartItem.Quantity += quantity; // เพิ่มจำนวนสินค้าในตะกร้า
        }
        else
        {
            cartItem = new CartItem
            {
                ProductId = productId,
                CustomerID = userEmail,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // อัพเดทจำนวนสินค้าในตะกร้า
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            return RedirectToAction("RemoveFromCart", new { cartItemId });
        }

        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            var product = await _context.Products.FindAsync(cartItem.ProductId);
            if (product != null && product.StockQuantity >= quantity)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "สินค้าหมดสต็อกหรือจำนวนที่ต้องการมากเกินไป";
            }
        }

        return RedirectToAction("Index");
    }

    // ลบสินค้าออกจากตะกร้า
    [HttpPost]
    public IActionResult RemoveFromCart(int cartItemId)
    {
        var cartItem = _context.CartItems.SingleOrDefault(c => c.CartItemId == cartItemId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // หน้าชำระเงิน
    public IActionResult Checkout()
    {
        var userEmail = _httpContextAccessor.HttpContext.User.Identity.Name;

        var cartItems = (from cart in _context.CartItems
                         join product in _context.Products on cart.ProductId equals product.Id
                         where cart.CustomerID == userEmail
                         select new CartItemDto
                         {
                             CartItemId = cart.CartItemId,
                             ProductId = product.Id,
                             ProductName = product.ProductName,
                             Price = product.Price,
                             Quantity = cart.Quantity,
                             Total = cart.Quantity * product.Price
                         }).ToList();

        var totalPrice = cartItems.Sum(item => item.Total);

        ViewBag.TotalPrice = totalPrice;
        return View(cartItems);
    }
}
