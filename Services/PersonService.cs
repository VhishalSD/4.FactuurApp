using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FactuurApp.Models;
using FactuurApp.Utilities;

namespace FactuurApp.Services
{
    // Service for managing people with input validation
    public class PersonService
    {
        // Readonly Private Fields 
        private readonly JsonFileHandler _jsonHandler;

        // Private Fields 
        private List<Person> _people;

        // Constants 
        private const string FilePath = "people.json";
        private const string SeparatorLine = "========================================";

        // Constructor 
        public PersonService(JsonFileHandler jsonHandler)
        {
            _jsonHandler = jsonHandler;
            _people = _jsonHandler.LoadFromJson<Person>(FilePath);
        }

        // Public Methods 

        // Save all data to JSON and exit
        public void SaveAndExit()
        {
            _jsonHandler.SaveToJson(FilePath, _people);
        }

        // Create a new person with input validation
        public void CreatePerson()
        {
            string? name = ReadValidatedName("Enter name (letters, spaces, hyphen; empty to cancel): ");
            if (name == null) return;

            DateTime? birthdate = ReadValidatedBirthdate("Enter birthdate (dd-MM-yyyy; empty to cancel)");
            if (birthdate == null) return;

            int newId = _people.Any() ? _people.Max(p => p.Id) + 1 : 1;
            _people.Add(new Person { Id = newId, Name = name, BirthDate = birthdate.Value });

            Console.WriteLine("Person added successfully.");
        }

        // Display all people
        public void ReadAllPersons()
        {
            if (!_people.Any())
            {
                Console.WriteLine("No people found.");
                return;
            }

            Console.WriteLine("\n--- All People ---");
            foreach (var p in _people)
                Console.WriteLine($"#{p.Id} - {p.Name} ({p.BirthDate:dd-MM-yyyy})");
        }

        // Search for people by name
        public void SearchByName()
        {
            Console.Write("Enter name to search: ");
            string name = Console.ReadLine()?.Trim() ?? "";

            var results = _people.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Any())
                results.ForEach(p => Console.WriteLine($"#{p.Id} - {p.Name} ({p.BirthDate:dd-MM-yyyy})"));
            else
                Console.WriteLine("No person found.");
        }

        // Update existing person details
        public void UpdatePerson()
        {
            Console.Write("Enter ID of person to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                Console.WriteLine("Person not found.");
                return;
            }

            string? newName = ReadValidatedName("New name (leave empty to keep current, cancel with empty input): ");
            if (newName != null)
                person.Name = newName;

            DateTime? newBirthdate = ReadValidatedBirthdate("New birthdate (dd-MM-yyyy; leave empty to keep current)");
            if (newBirthdate != null)
                person.BirthDate = newBirthdate.Value;

            Console.WriteLine("Person updated.");
        }

        // Confirm and delete a person by ID
        public void ConfirmDeletePerson()
        {
            Console.Write("Enter ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                Console.WriteLine("Person not found.");
                return;
            }

            Console.Write($"Are you sure you want to delete {person.Name}? (y/n): ");
            string confirm = Console.ReadLine()?.Trim().ToLower() ?? "n";
            if (confirm == "y")
            {
                _people.Remove(person);
                Console.WriteLine("Person deleted.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        // Show sorting menu for people
        public void SortPersonsMenu()
        {
            Console.WriteLine("\nSort by:");
            Console.WriteLine("1. Name (A-Z)");
            Console.WriteLine("2. Name (Z-A)");
            Console.WriteLine("3. BirthDate (oldest first)");
            Console.WriteLine("4. BirthDate (youngest first)");
            Console.Write("Choose option: ");

            string choice = Console.ReadLine()?.Trim() ?? "";

            switch (choice)
            {
                case "1":
                    _people = _people.OrderBy(p => p.Name).ToList();
                    Console.WriteLine("Sorted by Name (A-Z).");
                    break;
                case "2":
                    _people = _people.OrderByDescending(p => p.Name).ToList();
                    Console.WriteLine("Sorted by Name (Z-A).");
                    break;
                case "3":
                    _people = _people.OrderBy(p => p.BirthDate).ToList();
                    Console.WriteLine("Sorted by BirthDate (oldest first).");
                    break;
                case "4":
                    _people = _people.OrderByDescending(p => p.BirthDate).ToList();
                    Console.WriteLine("Sorted by BirthDate (youngest first).");
                    break;
                default:
                    Console.WriteLine("Invalid choice. No sorting applied.");
                    return;
            }

            ReadAllPersons();
        }

        // Show statistics about people
        public void ShowStatistics()
        {
            if (!_people.Any())
            {
                Console.WriteLine("No person to show statistics.");
                return;
            }

            int total = _people.Count;
            double avgAge = _people.Average(p => (DateTime.Now - p.BirthDate).TotalDays / 365.25);
            int youngestAge = (int)(_people.Max(p => p.BirthDate).Subtract(DateTime.Now).TotalDays / -365.25);
            int oldestAge = (int)(_people.Min(p => p.BirthDate).Subtract(DateTime.Now).TotalDays / -365.25);

            Console.WriteLine("\n--- People Statistics ---");
            Console.WriteLine($"Total people   : {total}");
            Console.WriteLine($"Average age    : {avgAge:F1} years");
            Console.WriteLine($"Youngest person: {youngestAge} years old");
            Console.WriteLine($"Oldest person  : {oldestAge} years old");
        }

        // === Private Methods ===

        // Validate and read name input
        private string? ReadValidatedName(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Name input cancelled.");
                    return null;
                }

                if (!Regex.IsMatch(input, @"^[\p{L}\p{M} \-]+$"))
                {
                    Console.WriteLine("Invalid name. Use letters, spaces or hyphen only.");
                    continue;
                }

                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                return ti.ToTitleCase(input.ToLower());
            }
        }

        // Validate and read birthdate input
        private DateTime? ReadValidatedBirthdate(string prompt)
        {
            DateTime earliestDate = DateTime.Now.AddYears(-120);
            DateTime latestDate = DateTime.Now;

            while (true)
            {
                Console.Write($"{prompt} (between {earliestDate:dd-MM-yyyy} and {latestDate:dd-MM-yyyy}): ");
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Birthdate input cancelled.");
                    return null;
                }

                if (!DateTime.TryParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthdate))
                {
                    Console.WriteLine("Invalid date format. Use dd-MM-yyyy.");
                    continue;
                }

                if (birthdate < earliestDate)
                {
                    Console.WriteLine($"Birthdate cannot be older than 120 years ({earliestDate:dd-MM-yyyy}).");
                    continue;
                }

                if (birthdate > latestDate)
                {
                    Console.WriteLine($"Birthdate cannot be in the future ({latestDate:dd-MM-yyyy}).");
                    continue;
                }

                return birthdate;
            }
        }
    }
}