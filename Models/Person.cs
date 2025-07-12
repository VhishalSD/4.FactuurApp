using System;

namespace JSONCRUD.Models
{
    // Model voor een persoon
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}
