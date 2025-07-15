using System;
using System.Collections.Generic;

namespace JSONCRUD.Models
{
    // Model representing an invoice
    public class Invoice
    {
        // Unique invoice ID
        public int Id { get; set; }

        // Customer's name
        public string CustomerName { get; set; } = string.Empty;

        // Date when the invoice was created
        public DateTime InvoiceDate { get; set; }

        // List of line items on the invoice
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        // Calculate total amount excluding VAT
        public decimal TotalAmount
        {
            get
            {
                decimal total = 0m;
                foreach (var item in Items)
                {
                    total += item.Quantity * item.UnitPrice;
                }
                return total;
            }
        }

        // Calculate VAT amount at 21%
        public decimal VATAmount => TotalAmount * 0.21m;

        // Calculate total amount including VAT
        public decimal TotalWithVAT => TotalAmount + VATAmount;
    }
}
