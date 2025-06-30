using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class ProductFilterViewModel
    {
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        
        [Display(Name = "Min Price")]
        public decimal? MinPrice { get; set; }
        
        [Display(Name = "Max Price")]
        public decimal? MaxPrice { get; set; }
        
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Search")]
        public string? SearchString { get; set; }
        
        // For pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        // Lists for view rendering
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
    }
} 