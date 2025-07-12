using System;
using System.Collections.Generic;

namespace JSONCRUD.Models
{
    // Model voor een factuur
    public class Invoice
    {
        public int Id { get; set; }  // Unieke factuur-ID
        public string CustomerName { get; set; } = string.Empty;  // Naam van klant
        public DateTime InvoiceDate { get; set; }  // Datum factuur
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();  // Lijst met factuurregels

        // Bereken totaal exclusief BTW
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

        // Bereken BTW (21%)
        public decimal VATAmount => TotalAmount * 0.21m;

        // Totaal inclusief BTW
        public decimal TotalWithVAT => TotalAmount + VATAmount;
    }
}
