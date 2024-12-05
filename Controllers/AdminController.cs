using Microsoft.AspNetCore.Authorization; // ใช้สำหรับการตรวจสอบสิทธิ์
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PakinProject.Data;
using PakinProject.Models;

[Authorize] // บังคับให้ต้อง Login ก่อนถึงจะเข้าถึง Controller ได้
public class AdminController : Controller
{
    private readonly PakinProjectContext _context;

    public AdminController(PakinProjectContext context)
    {
        _context = context;
    }

    // ตรวจสอบ Role ว่าเป็น Admin หรือไม่
    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    public async Task<IActionResult> Index(string category, string sortOption)
    {
        if (!IsAdmin())
        {
            return Forbid(); // ป้องกันไม่ให้ User ที่ไม่ใช่ Admin เข้าถึง
        }

        var products = _context.Products.AsQueryable();

        // กรองสินค้าตามหมวดหมู่
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category == category);
        }

        // จัดเรียงสินค้าตามตัวเลือก
        products = sortOption switch
        {
            "PriceAsc" => products.OrderBy(p => p.Price),
            "PriceDesc" => products.OrderByDescending(p => p.Price),
            "Popular" => products.OrderByDescending(p => p.Sales),
            _ => products
        };

        return View(await products.ToListAsync());
    }

    public IActionResult Create()
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        ViewBag.Categories = new SelectList(new[] { "Mobile", "PC", "Console", "Accessories" });
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile imageFile)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    product.ImageBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(new[] { "Mobile", "PC", "Console", "Accessories" });
        return View(product);
    }

    public IActionResult LowStockAlert()
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var lowStockProducts = _context.Products.Where(p => p.StockQuantity <= 10).ToList();
        return View(lowStockProducts);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        ViewBag.Categories = new SelectList(new[] { "Mobile", "PC", "Console", "Accessories" });
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            existingProduct.ProductCode = product.ProductCode;
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;
            existingProduct.StockQuantity = product.StockQuantity;

            if (product.UploadedImage != null && product.UploadedImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await product.UploadedImage.CopyToAsync(memoryStream);
                    existingProduct.ImageBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(new[] { "Mobile", "PC", "Console", "Accessories" });
        return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
