using System;

namespace JSONCRUD.Models
{
    // Person model representing an individual with an ID, name, and birthdate
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}
