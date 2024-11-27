using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PakinProject.Data;
using PakinProject.Models;

public class AdminController : Controller
{
    private readonly PakinProjectContext _context;

    public AdminController(PakinProjectContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string category, string sortOption)
    {
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
            "Popular" => products.OrderByDescending(p => p.Sales), // ตัวอย่าง ใช้ฟิลด์ Sales
            _ => products
        };

        return View(await products.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(new[] { "Smartphone", "Gaming Gear", "Accessories" });
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile imageFile)
    {
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
        ViewBag.Categories = new SelectList(new[] { "Smartphone", "Gaming Gear", "Accessories" });
        return View(product);
    }

    public IActionResult LowStockAlert()
    {
        var lowStockProducts = _context.Products.Where(p => p.StockQuantity <= 10).ToList();
        return View(lowStockProducts);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // ส่งรายการหมวดหมู่ไปยัง View ผ่าน ViewBag
        ViewBag.Categories = new SelectList(new[] { "Smartphone", "Gaming Gear", "Accessories" });

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (ModelState.IsValid)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            // อัปเดตข้อมูลสินค้า
            existingProduct.ProductCode = product.ProductCode;
            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;
            existingProduct.StockQuantity = product.StockQuantity;

            // อัปเดตรูปภาพหากมีการอัปโหลดใหม่
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
                await _context.SaveChangesAsync(); // บันทึกการเปลี่ยนแปลงลงในฐานข้อมูล
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index)); // กลับไปที่หน้ารายการสินค้า
        }

        // ส่งรายการหมวดหมู่ไปยัง View อีกครั้งในกรณีที่ ModelState ไม่ถูกต้อง
        ViewBag.Categories = new SelectList(new[] { "Smartphone", "Gaming Gear", "Accessories" });
        return View(product);
    }

    // ฟังก์ชันตรวจสอบว่าสินค้าด้วย ID มีอยู่หรือไม่
    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    // --- ฟังก์ชัน Delete ---
    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
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
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // --- ฟังก์ชัน Details ---
    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
