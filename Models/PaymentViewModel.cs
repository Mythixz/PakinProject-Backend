using System.ComponentModel.DataAnnotations;

namespace PakinProject.Models
{
    public class PaymentViewModel
    {
        public PaymentViewModel()
        {
            PaymentMethods = new List<string> { "CreditCard", "Wallet", "COD" };
        }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string Address { get; set; }

        public List<string> PaymentMethods { get; set; }

        [Required(ErrorMessage = "Please select a payment method.")]
        public string SelectedPaymentMethod { get; set; }

        [CreditCard(ErrorMessage = "Invalid credit card number.")]
        public string CreditCardNumber { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiry date must be in MM/YY format.")]
        public string CreditCardExpiry { get; set; }

        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be 3 digits.")]
        public string CreditCardCVV { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Wallet balance must be a positive value.")]
        public decimal WalletBalance { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please select a delivery option.")]
        public string DeliveryOption { get; set; }
    }
}
