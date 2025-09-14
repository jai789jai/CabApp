using CabApp.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation
{
    public class DataStore: IDataStore
    {
        private string _directoryPath;
        private readonly IAppLogger _appLogger;
        private readonly JsonSerializerOptions _jsonOptions;

        public DataStore(string directoryPath, IAppLogger appLogger)
        {
            _directoryPath = directoryPath;
            _appLogger = appLogger;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null, // Keep original property names
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };
        }

        public async Task<List<T>> GetData<T>(string storeName)
        {
            List<T> data = new List<T>();
            try
            {
                string dataPath = GetFilePath(storeName);

                if (!File.Exists(dataPath))
                {
                    return data;
                }

                var rawData = await File.ReadAllTextAsync(dataPath);
                data = JsonSerializer.Deserialize<List<T>>(rawData, _jsonOptions) ?? new List<T>();
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"Exception occurred in GetData for type {typeof(T).Name}: {ex.Message}", ex);
            }
            return data;
        }

        public async Task WriteData<T>(List<T> data, string storeName)
        {
            try
            {
                string dataPath = GetFilePath(storeName);
                string serializedData = JsonSerializer.Serialize(data, _jsonOptions);

                if(string.IsNullOrWhiteSpace(serializedData))   
                    return;

                // Ensure the directory exists before writing the file
                string directory = Path.GetDirectoryName(dataPath) ?? string.Empty;
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(dataPath, serializedData);
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"Exception occurred in WriteData for {typeof(T).Name}", ex);
            }
        }

        public void ClearData(string storeName)
        {
            try
            {
                string dataPath = GetFilePath(storeName);
                if (File.Exists(dataPath))
                {
                    File.Delete(dataPath);
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"Exception occurred in ClearData for {storeName}", ex);
            }
        }

        private string GetFilePath(string storeName)
        {
            string dataPath = Path.Combine(_directoryPath, storeName + ".json");
            return dataPath;
        }

        private string CleanJsonData(string jsonData)
        {
            try
            {
                // Remove any BOM or invisible characters
                jsonData = jsonData.Trim('\uFEFF', '\u200B');
                
                // Remove any trailing commas
                jsonData = System.Text.RegularExpressions.Regex.Replace(jsonData, @",(\s*[}\]])", "$1");
                
                // Ensure proper JSON array format
                if (!jsonData.TrimStart().StartsWith("["))
                {
                    jsonData = "[" + jsonData + "]";
                }
                
                return jsonData;
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"Error cleaning JSON data: {ex.Message}", ex);
                return jsonData; // Return original if cleaning fails
            }
        }
    }
}
