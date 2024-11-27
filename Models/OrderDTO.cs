namespace PakinProject.Models
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string CustomerID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; } // เพิ่มชื่อสินค้า
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
