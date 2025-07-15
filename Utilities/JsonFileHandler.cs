using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FactuurApp.Utilities;

namespace FactuurApp.Utilities
{
    // Class for reading and writing JSON files
    public class JsonFileHandler
    {
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
                Console.WriteLine($"Error loading JSON from '{filePath}': {ex.Message}");
                return new List<T>();
            }
        }

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
                Console.WriteLine($"Error saving JSON to '{filePath}': {ex.Message}");
            }
        }
    }
}
