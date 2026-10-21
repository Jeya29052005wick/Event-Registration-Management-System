
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventRegistrationManagementSystem.Utilities
{
    public class JsonFileHelper
    {
        private readonly string _dataFolder;
        private readonly JsonSerializerOptions _options;

        public JsonFileHelper()
        {
            _dataFolder = Path.Combine(
                AppContext.BaseDirectory, "Data");

            Directory.CreateDirectory(_dataFolder);

            _options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _options.Converters.Add(new JsonStringEnumConverter());
        }

        public List<T> Load<T>(string fileName)
        {
            string path = Path.Combine(_dataFolder, fileName);

            if (!File.Exists(path))
                return new List<T>();

            try
            {
                string json = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(json))
                    return new List<T>();

                return JsonSerializer.Deserialize<List<T>>(
                    json, _options) ?? new List<T>();
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException(
                    $"The JSON file '{fileName}' is invalid. " +
                    "Back up and repair the file before continuing.", ex);
            }
        }

        public void Save<T>(string fileName, List<T> data)
        {
            Directory.CreateDirectory(_dataFolder);

            string path = Path.Combine(_dataFolder, fileName);
            string tempPath = path + ".tmp";

            string json = JsonSerializer.Serialize(data, _options);

            try
            {
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, path, true);
            }
            finally
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
    }
}
