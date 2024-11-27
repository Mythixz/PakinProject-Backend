using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using PakinProject.Models;
using PakinProject.Data; // เพิ่มการนำเข้าที่เหมาะสม
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore; // ใช้สำหรับ LINQ

namespace PakinProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
