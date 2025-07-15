using FactuurApp.Models;
using System;
using System.Collections.Generic;

namespace FactuurApp.Models
{
    // Model for an invoice
    public class Invoice
    {
        public int Id { get; set; }  // Unique invoice ID
        public string CustomerName { get; set; } = string.Empty;  // Customer's name
        public DateTime InvoiceDate { get; set; }  // Invoice date
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();  // List of invoice items

        // Calculate total excluding VAT
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

        // Calculate VAT (21%)
        public decimal VATAmount => TotalAmount * 0.21m;

        // Total including VAT
        public decimal TotalWithVAT => TotalAmount + VATAmount;
    }
}
