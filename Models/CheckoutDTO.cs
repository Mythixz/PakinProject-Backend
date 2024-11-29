namespace PakinProject.Models
{
    public class CheckoutDTO
    {
        public int UserId { get; set; } // เพิ่ม UserId
        public string UserEmail { get; set; }
        public List<CartItemDto> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
    }


}
