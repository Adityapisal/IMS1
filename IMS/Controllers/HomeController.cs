using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Data;
using System.Linq;

namespace InventoryManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Get dashboard statistics
        ViewBag.TotalProducts = await _context.Products.CountAsync();
        ViewBag.TotalCategories = await _context.Categories.CountAsync();
        ViewBag.LowStockCount = await _context.Products.CountAsync(p => p.Quantity < 10); // Assuming low stock is < 10
        
        // Fix for SQLite decimal sum issue - perform calculation in memory
        var products = await _context.Products.ToListAsync();
        decimal totalValue = products.Sum(p => p.Price * p.Quantity);
        ViewBag.TotalValue = totalValue.ToString("F2");

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
