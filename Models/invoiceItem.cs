namespace JSONCRUD.Models
{
    // Model voor een factuurregel (item)
    public class InvoiceItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice; // Totaalprijs per regel
    }
}
