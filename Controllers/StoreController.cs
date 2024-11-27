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

        // แสดงสินค้าทั้งหมด
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // ดึงข้อมูลสินค้าทั้งหมด
            return View(products);
        }

        // แสดงรายละเอียดสินค้า
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            return View(product);
        }
    }
}
