using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using JSONCRUD.Models;
using JSONCRUD.Utilities;

namespace JSONCRUD.Services
{
    // Service voor beheer van personen met valideerbare invoer
    public class PersonService
    {
        private readonly JsonFileHandler _jsonHandler;
        private List<Person> _people;
        private const string FilePath = "people.json";

        public PersonService(JsonFileHandler jsonHandler)
        {
            _jsonHandler = jsonHandler;
            _people = _jsonHandler.LoadFromJson<Person>(FilePath);
        }

        public void SaveAndExit()
        {
            _jsonHandler.SaveToJson(FilePath, _people);
        }

        // Hoofdletters per woord zetten en valideer naam
        private string? ReadValidatedName(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Name input cancelled.");
                    return null; // annuleren
                }

                // Regex laat letters incl. accenten, spaties en koppelteken toe
                if (!Regex.IsMatch(input, @"^[\p{L}\p{M} \-]+$"))
                {
                    Console.WriteLine("Invalid name. Use letters, spaces or hyphen only.");
                    continue;
                }

                // Zet hoofdletters per woord
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                string formattedName = ti.ToTitleCase(input.ToLower());

                return formattedName;
            }
        }

        // Geboortedatum in dd-MM-yyyy, max 120 jaar oud
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
                    return null; // annuleren
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

        // Persoon aanmaken met validatie en mogelijkheid annuleren
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

            string? newName = ReadValidatedName("New name (leave empty to keep current, empty input cancels change): ");
            if (newName != null)
                person.Name = newName;

            DateTime? newBirthdate = ReadValidatedBirthdate("New birthdate (dd-MM-yyyy) (leave empty to keep current, empty input cancels change)");
            if (newBirthdate != null)
                person.BirthDate = newBirthdate.Value;

            Console.WriteLine("Person updated.");
        }

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

            ReadAllPersons(); // Toon direct de gesorteerde lijst
        }

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
    }
}
