namespace JSONCRUD.Models
{
    // Model representing a single invoice line item
    public class InvoiceItem
    {
        // Description of the item or service
        public string Description { get; set; } = string.Empty;

        // Quantity of the item
        public int Quantity { get; set; }

        // Price per unit
        public decimal UnitPrice { get; set; }

        // Total price for this line (quantity * unit price)
        public decimal SubTotal => Quantity * UnitPrice;
    }
}
