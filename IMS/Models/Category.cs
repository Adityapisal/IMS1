using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
        
        // Navigation property for related products
        public ICollection<Product>? Products { get; set; }
    }
} 