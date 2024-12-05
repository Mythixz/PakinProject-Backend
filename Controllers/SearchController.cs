using Microsoft.AspNetCore.Mvc;
using PakinProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class SearchController : Controller
{
    private readonly List<Product> _products = new List<Product>
    {
        new Product { Id = 1, ProductName = "Laptop", Description = "High performance laptop", Price = 1500, ImageBase64 = "" },
        new Product { Id = 2, ProductName = "Mouse", Description = "Wireless mouse", Price = 20, ImageBase64 = "" },
        new Product { Id = 3, ProductName = "Keyboard", Description = "Mechanical keyboard", Price = 50, ImageBase64 = "" },
    };

    [HttpGet]
    public IActionResult Index(string keyword, string sortBy)
    {
        var filteredProducts = _products;

        // ค้นหาตาม keyword
        if (!string.IsNullOrEmpty(keyword))
        {
            filteredProducts = filteredProducts
                .Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // จัดเรียงตาม sortBy
        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy)
            {
                case "price_asc":
                    filteredProducts = filteredProducts.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Price).ToList();
                    break;
                case "popularity":
                    // ตัวอย่างการเรียงตามความนิยม (ยังไม่มีฟิลด์ popularity)
                    // filteredProducts = filteredProducts.OrderByDescending(p => p.Popularity).ToList();
                    break;
                default:
                    break;
            }
        }

        // ส่งค่า ViewData สำหรับใช้แสดงใน View
        ViewData["CurrentKeyword"] = keyword;
        ViewData["CurrentSortBy"] = sortBy;

        return View(filteredProducts);
    }
}
