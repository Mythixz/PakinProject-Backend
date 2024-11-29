namespace PakinProject.Models
{
public class CartItemDto
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; } // แก้ให้สามารถกำหนดค่าได้
}

}
