using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace JSONCRUD.Utilities
{
    // Utility class for reading and writing JSON files
    public class JsonFileHandler
    {
        private const string SeparatorLine = "========================================";

        // Load list from JSON file
        public List<T> LoadFromJson<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<T>();

                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(SeparatorLine);
                Console.WriteLine($"Error loading JSON from '{filePath}': {ex.Message}");
                Console.WriteLine(SeparatorLine);
                Console.ResetColor();
                return new List<T>();
            }
        }

        // Save list to JSON file
        public void SaveToJson<T>(string filePath, List<T> data)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(SeparatorLine);
                Console.WriteLine($"Error saving JSON to '{filePath}': {ex.Message}");
                Console.WriteLine(SeparatorLine);
                Console.ResetColor();
            }
        }
    }
}
