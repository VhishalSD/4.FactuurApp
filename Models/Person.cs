using System;

namespace FactuurApp.Models
{
    // This class represents a person, used to link invoices to individuals.
    public class Person
    {
        // Unique identifier for the person
        public int Id { get; set; }

        // Full name of the person
        public string Name { get; set; } = string.Empty;

        // Date of birth of the person
        public DateTime BirthDate { get; set; }
    }
}
