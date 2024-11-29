using PakinProject.Data;
using PakinProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace CMM349FINAL.Views.Admin
{
    public class IndexModel : PageModel
    {
        private readonly PakinProjectContext _context;

        // เพิ่ม Property สำหรับเก็บข้อมูลสินค้า
        public IList<Product> Products { get; set; }

        public IndexModel(PakinProjectContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            // ดึงข้อมูลสินค้าจากฐานข้อมูล
            Products = _context.Products.ToList(); // ใช้ _context เพื่อดึงข้อมูล
            ViewData["Title"] = "Admin Product List";
        }
    }
}
