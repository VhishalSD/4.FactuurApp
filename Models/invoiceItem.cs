namespace FactuurApp.Models
{
    // Model for an invoice line item
    public class InvoiceItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice; // Total price for the item
    }
}
