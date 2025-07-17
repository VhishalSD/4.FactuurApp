using System;
using System.Globalization;
using FactuurApp.Services;
using FactuurApp.Utilities;

namespace FactuurApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set default culture to Dutch (Netherlands) for euro symbol support and date formatting
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("nl-NL");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("nl-NL");

            var jsonFileHandler = new JsonFileHandler();
            var personService = new PersonService(jsonFileHandler);
            var invoiceService = new InvoiceService(jsonFileHandler);

            while (true)
            {
                ShowMainMenu();
                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        ShowPersonMenu(personService);
                        break;
                    case "2":
                        ShowInvoiceMenu(invoiceService);
                        break;
                    case "0":
                        personService.SaveAndExit();
                        invoiceService.SaveAndExit();
                        Console.WriteLine();
                        Console.WriteLine("Saving and exiting... Goodbye!");
                        Environment.Exit(0);
                        break;
                    default:
                        ShowError("Invalid option, please try again.");
                        break;
                }
            }
        }

        // Display the main menu
        static void ShowMainMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== FACTUURBEHEER SYSTEEM ===");
            Console.ResetColor();
            Console.WriteLine("1. Manage Persons");
            Console.WriteLine("2. Manage Invoices");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("Choose an option: ");
            Console.WriteLine();
        }

        // Show the person management submenu
        static void ShowPersonMenu(PersonService service)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n--- People Management ---");
                Console.WriteLine("1. Add Person");
                Console.WriteLine("2. View All People");
                Console.WriteLine("3. Search Person");
                Console.WriteLine("4. Update Person");
                Console.WriteLine("5. Delete Person");
                Console.WriteLine("6. Sort People");
                Console.WriteLine("7. Show Statistics");
                Console.WriteLine("0. Back to Main Menu");
                Console.ResetColor();
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": service.CreatePerson(); break;
                    case "2": service.ReadAllPersons(); break;
                    case "3": service.SearchByName(); break;
                    case "4": service.UpdatePerson(); break;
                    case "5": service.ConfirmDeletePerson(); break;
                    case "6": service.SortPersonsMenu(); break;
                    case "7": service.ShowStatistics(); break;
                    case "0": return;
                    default: ShowError("Invalid input."); break;
                }
            }
        }

        // Show the invoice management submenu
        static void ShowInvoiceMenu(InvoiceService service)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n--- Invoice Management ---");
                Console.WriteLine("1. Create Invoice");
                Console.WriteLine("2. View All Invoices");
                Console.WriteLine("3. Search Invoice by ID");
                Console.WriteLine("4. Delete Invoice");
                Console.WriteLine("0. Back to Main Menu");
                Console.ResetColor();
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": service.CreateInvoice(); break;
                    case "2": service.ReadAllInvoices(); break;
                    case "3": service.SearchInvoiceById(); break;
                    case "4": service.DeleteInvoice(); break;
                    case "0": return;
                    default: ShowError("Invalid input."); break;
                }
            }
        }

        // Display error messages in red color
        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {message}");
            Console.ResetColor();
        }
    }
}
