using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FactuurApp.Utilities;

namespace FactuurApp.Utilities
{
    // Utility class responsible for reading and writing JSON files
    public class JsonFileHandler
    {
        // Loads a list of objects of type T from a JSON file
        public List<T> LoadFromJson<T>(string filePath)
        {
            try
            {
                // Return empty list if file does not exist
                if (!File.Exists(filePath))
                    return new List<T>();

                // Read JSON content from file
                string jsonString = File.ReadAllText(filePath);

                // Deserialize the JSON into a list of T
                return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
            }
            catch (Exception ex)
            {
                // Log error and return empty list on failure
                Console.WriteLine($"Error loading JSON from '{filePath}': {ex.Message}");
                return new List<T>();
            }
        }

        // Saves a list of objects of type T to a JSON file
        public void SaveToJson<T>(string filePath, List<T> data)
        {
            try
            {
                // Configure options for readable formatting
                var options = new JsonSerializerOptions { WriteIndented = true };

                // Serialize the data to JSON format
                string jsonString = JsonSerializer.Serialize(data, options);

                // Write the JSON string to the specified file
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                // Log error if saving fails
                Console.WriteLine($"Error saving JSON to '{filePath}': {ex.Message}");
            }
        }
    }
}
