namespace PakinProject.Models
{
    public class InvoiceDTO
    {
        public string OrderId { get; set; }
        public string CustomerID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}
