using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }
        
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Product Image 1")]
        [StringLength(500)]
        public string? ProductImage1 { get; set; }
        
        [Display(Name = "Product Image 2")]
        [StringLength(500)]
        public string? ProductImage2 { get; set; }
        
        [Display(Name = "Product Image 3")]
        [StringLength(500)]
        public string? ProductImage3 { get; set; }
        
        [NotMapped]
        public bool IsLowStock => Quantity < 5;
    }
} 