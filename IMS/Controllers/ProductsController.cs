using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace InventoryManagementSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(ProductFilterViewModel filter)
        {
            // Initialize filter if it's null
            filter ??= new ProductFilterViewModel();
            
            // Set default page size if not specified
            if (filter.PageSize <= 0) filter.PageSize = 10;
            if (filter.PageNumber <= 0) filter.PageNumber = 1;
            
            // Get categories for the dropdown
            filter.Categories = await _context.Categories.ToListAsync();
            
            // Start with all products including their Category
            var query = _context.Products.Include(p => p.Category).AsQueryable();
            
            // Apply filters
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }
            
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }
            
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }
            
            if (filter.StartDate.HasValue)
            {
                query = query.Where(p => p.CreatedDate >= filter.StartDate.Value);
            }
            
            if (filter.EndDate.HasValue)
            {
                query = query.Where(p => p.CreatedDate <= filter.EndDate.Value);
            }
            
            if (!string.IsNullOrEmpty(filter.SearchString))
            {
                query = query.Where(p => p.ProductName.Contains(filter.SearchString) || 
                                         p.Category.Name.Contains(filter.SearchString));
            }
            
            // Order by creation date (newest first)
            query = query.OrderByDescending(p => p.CreatedDate);
            
            // Apply pagination
            var paginatedProducts = await PaginatedList<Product>.CreateAsync(query, filter.PageNumber, filter.PageSize);
            
            ViewBag.PaginatedProducts = paginatedProducts;
            return View(filter);
        }
        
        // GET: Products/ExportToExcel
        public async Task<IActionResult> ExportToExcel()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Products");
            
            // Header row
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Product Name";
            worksheet.Cells[1, 3].Value = "Category";
            worksheet.Cells[1, 4].Value = "Price";
            worksheet.Cells[1, 5].Value = "Quantity";
            worksheet.Cells[1, 6].Value = "Created Date";
            
            // Format the header row
            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                range.Style.Font.Color.SetColor(Color.Black);
            }
            
            // Data rows
            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cells[row, 1].Value = product.Id;
                worksheet.Cells[row, 2].Value = product.ProductName;
                worksheet.Cells[row, 3].Value = product.Category?.Name;
                worksheet.Cells[row, 4].Value = product.Price;
                worksheet.Cells[row, 5].Value = product.Quantity;
                worksheet.Cells[row, 6].Value = product.CreatedDate.ToString("yyyy-MM-dd");
                
                // Highlight low stock items
                if (product.IsLowStock)
                {
                    worksheet.Cells[row, 5].Style.Font.Color.SetColor(Color.Red);
                    worksheet.Cells[row, 5].Style.Font.Bold = true;
                }
                
                row++;
            }
            
            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
            
            // Format price column
            worksheet.Column(4).Style.Numberformat.Format = "$#,##0.00";
            
            // Create a memory stream to hold the Excel file
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            
            // Return the Excel file
            string fileName = "Products_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,CategoryId,Price,Quantity,CreatedDate,ProductImage1,ProductImage2,ProductImage3")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,CategoryId,Price,Quantity,CreatedDate,ProductImage1,ProductImage2,ProductImage3")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Product updated successfully";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product deleted successfully";
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
} 