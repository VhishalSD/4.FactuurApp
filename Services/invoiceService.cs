using JSONCRUD.Models;
using JSONCRUD.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JSONCRUD.Services
{
    // Service for managing invoices with input validation
    public class InvoiceService
    {
        // Readonly Private Fields
        private readonly JsonFileHandler _jsonHandler;

        // Private Fields
        private List<Invoice> _invoices;

        // Constants
        private const string FilePath = "invoices.json";
        private readonly CultureInfo _nlCulture = new CultureInfo("nl-NL");
        private const string SeparatorLine = "========================================";

        // Constructor 
        public InvoiceService(JsonFileHandler jsonHandler)
        {
            _jsonHandler = jsonHandler;
            _invoices = _jsonHandler.LoadFromJson<Invoice>(FilePath);
        }

        // Public Methods 

        // Save invoices to JSON file and show confirmation
        public void SaveAndExit()
        {
            _jsonHandler.SaveToJson(FilePath, _invoices);
            Console.WriteLine("Invoices saved.");
        }

        // Create a new invoice with input validation
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
                ShowError("Invalid name. Only letters (including accents), spaces and hyphens allowed.");
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
                    ShowError("Invalid quantity. Must be a positive integer.");
                }

                decimal unitPrice;
                while (true)
                {
                    Console.Write("Unit price (e.g. 12,50): ");
                    if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, _nlCulture, out unitPrice) && unitPrice >= 0)
                        break;
                    ShowError("Invalid unit price. Must be a non-negative number.");
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
                ShowError("Invalid ID.");
                return;
            }

            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null)
            {
                ShowError("Invoice not found.");
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

        // Search and display an invoice by ID
        public void SearchInvoiceById()
        {
            Console.Write("Enter invoice ID to search: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                ShowError("Invalid ID.");
                return;
            }

            var invoice = _invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null)
            {
                ShowError("Invoice not found.");
                return;
            }

            DisplayInvoice(invoice);
        }

        // Private Methods 

        // Display formatted invoice details
        private void DisplayInvoice(Invoice inv)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(SeparatorLine);
            Console.WriteLine($"Invoice ID   : {inv.Id}");
            Console.WriteLine($"Customer     : {inv.CustomerName}");
            Console.WriteLine($"Date         : {inv.InvoiceDate:dd-MM-yyyy}");
            Console.WriteLine("Items:");
            foreach (var item in inv.Items)
            {
                Console.WriteLine($"- {item.Description}: {item.Quantity} x {item.UnitPrice.ToString("C", _nlCulture)} = {item.SubTotal.ToString("C", _nlCulture)}");
            }
            Console.WriteLine($"Subtotal     : {inv.TotalAmount.ToString("C", _nlCulture)}");
            Console.WriteLine($"VAT (21%)    : {inv.VATAmount.ToString("C", _nlCulture)}");
            Console.WriteLine($"Total incl.  : {inv.TotalWithVAT.ToString("C", _nlCulture)}");
            Console.WriteLine(SeparatorLine);
            Console.ResetColor();
        }

        // Validate customer name
        private bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-') return false;
            }
            return true;
        }

        // Capitalize customer name
        private string CapitalizeName(string name)
        {
            TextInfo textInfo = _nlCulture.TextInfo;
            return textInfo.ToTitleCase(name.ToLower());
        }

        // Show formatted error message
        private void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
