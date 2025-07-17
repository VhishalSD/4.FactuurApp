using FactuurApp.Models;
using FactuurApp.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FactuurApp.Services
{
    // Service to manage invoices, including creation, reading, searching and deletion
    public class InvoiceService
    {
        private readonly JsonFileHandler _jsonHandler;
        private List<Invoice> _invoices;
        private const string FilePath = "invoices.json";

        // Constructor loads invoices from JSON file
        public InvoiceService(JsonFileHandler jsonHandler)
        {
            _jsonHandler = jsonHandler;
            _invoices = _jsonHandler.LoadFromJson<Invoice>(FilePath);
        }

        // Save all invoices to JSON file
        public void SaveAndExit()
        {
            _jsonHandler.SaveToJson(FilePath, _invoices);
            Console.WriteLine("Invoices saved.");
        }

        // Create a new invoice with customer name and multiple items
        public void CreateInvoice()
        {
            string name;
            while (true)
            {
                Console.Write("Customer name: ");
                name = Console.ReadLine()?.Trim() ?? "";
                if (IsValidName(name))
                {
                    name = CapitalizeName(name);
                    break;
                }
                Console.WriteLine("Invalid name. Only letters (including accents), spaces and hyphens allowed.");
            }

            var items = new List<InvoiceItem>();
            while (true)
            {
                Console.Write("Item description (or enter to finish): ");
                string desc = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(desc)) break;

                int quantity;
                while (true)
                {
                    Console.Write("Quantity: ");
                    if (int.TryParse(Console.ReadLine(), out quantity) && quantity > 0)
                        break;
                    Console.WriteLine("Invalid quantity. Must be a positive integer.");
                }

                decimal unitPrice;
                while (true)
                {
                    Console.Write("Unit price (e.g. 12,50): ");
                    if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.CurrentCulture, out unitPrice) && unitPrice >= 0)
                        break;
                    Console.WriteLine("Invalid unit price. Must be a non-negative number.");
                }

                items.Add(new InvoiceItem
                {
                    Description = desc,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            int newId = _invoices.Any() ? _invoices.Max(i => i.Id) + 1 : 1;
            var invoice = new Invoice
            {
                Id = newId,
                CustomerName = name,
                InvoiceDate = DateTime.Now,
                Items = items
            };

            _invoices.Add(invoice);
            Console.WriteLine("Invoice created.");
        }

        // Display all invoices
        public void ReadAllInvoices()
        {
            if (!_invoices.Any())
            {
                Console.WriteLine("No invoices found.");
                return;
            }

            foreach (var inv in _invoices)
            {
                DisplayInvoice(inv);
            }
        }

        // Delete an invoice by ID with confirmation
        public void DeleteInvoice()
        {
            Console.Write("Enter invoice ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null)
            {
                Console.WriteLine("Invoice not found.");
                return;
            }

            Console.Write($"Are you sure you want to delete invoice #{id}? (y/n): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "n";
            if (confirm == "y")
            {
                _invoices.Remove(invoice);
                Console.WriteLine("Invoice deleted.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        // Search and display invoice by ID
        public void SearchInvoiceById()
        {
            Console.Write("Enter invoice ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null)
            {
                Console.WriteLine("Invoice not found.");
                return;
            }

            DisplayInvoice(invoice);
        }

        // Private helper to display invoice details with euro formatting
        private void DisplayInvoice(Invoice inv)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"Invoice ID   : {inv.Id}");
            Console.WriteLine($"Customer     : {inv.CustomerName}");
            Console.WriteLine($"Date         : {inv.InvoiceDate:dd-MM-yyyy}");
            Console.WriteLine("Items:");
            foreach (var item in inv.Items)
            {
                Console.WriteLine($"- {item.Description}: {item.Quantity} x {item.UnitPrice.ToString("C", CultureInfo.CurrentCulture)} = {item.SubTotal.ToString("C", CultureInfo.CurrentCulture)}");
            }
            Console.WriteLine($"Subtotal     : {inv.TotalAmount.ToString("C", CultureInfo.CurrentCulture)}");
            Console.WriteLine($"VAT (21%)    : {inv.VATAmount.ToString("C", CultureInfo.CurrentCulture)}");
            Console.WriteLine($"Total incl.  : {inv.TotalWithVAT.ToString("C", CultureInfo.CurrentCulture)}");
            Console.WriteLine(new string('=', 40));
            Console.ResetColor();
        }

        // Validate customer name input (letters, spaces, hyphen)
        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-') return false;
            }
            return true;
        }

        // Capitalize customer name to title case
        private string CapitalizeName(string name)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(name.ToLower());
        }
    }
}
