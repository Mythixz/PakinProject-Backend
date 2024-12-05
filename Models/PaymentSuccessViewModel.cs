namespace PakinProject.Models
{
    public class PaymentSuccessViewModel
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string Address { get; set; }
    }
}
