using JSONCRUD.Services;
using JSONCRUD.Utilities;
using System;

class Program
{
    static void Main()
    {
        // Initialiseer services met json-bestandshandler
        var personService = new PersonService(new JsonFileHandler());
        var invoiceService = new InvoiceService(new JsonFileHandler());

        while (true)
        {
            ShowMainMenu();
            HandleMainMenuInput(personService, invoiceService);
        }
    }

    // Hoofdmenu tonen
    static void ShowMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== MAIN MENU ===");
        Console.WriteLine("1. Manage People");
        Console.WriteLine("2. Manage Invoices");
        Console.WriteLine("0. Exit");
        Console.ResetColor();
        Console.Write("Choose an option: ");
    }

    // Hoofdmenu input verwerken
    static void HandleMainMenuInput(PersonService personService, InvoiceService invoiceService)
    {
        string? input = Console.ReadLine();

        switch (input)
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
                Environment.Exit(0);
                break;
            default:
                ShowError("Invalid choice.");
                break;
        }
    }

    // Personenbeheer menu tonen en verwerken
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

    // Factuurbeheer menu tonen en verwerken
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

    // Foutmelding tonen
    static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
