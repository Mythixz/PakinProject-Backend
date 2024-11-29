using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }  // เปลี่ยนเป็น Id แทน ProductId
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageBase64 { get; set; }
        [NotMapped]
        public IFormFile? UploadedImage { get; set; }

        public string Category { get; set; }
        public int StockQuantity { get; set; } // จำนวนสินค้า
        public int Sales { get; set; } // สำหรับจัดเรียงตามความนิยม
    }
}
