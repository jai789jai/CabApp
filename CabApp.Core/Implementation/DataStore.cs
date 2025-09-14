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
                WriteIndented = true
            };
        }

        public async Task<List<T>> GetData<T>(string storeName)
        {
            List<T> data = new List<T>();
            try
            {
                string dataPath = GetFilePath(storeName);

                if (!Directory.Exists(dataPath))
                    return data;

                var rawData = await File.ReadAllTextAsync(dataPath);
                data = JsonSerializer.Deserialize<List<T>>(dataPath, _jsonOptions) ?? new List<T>();

                _appLogger.LogInfo($"Data read successfully for type :{typeof(T).Name}");

            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occured in GetData", ex);
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

                await File.WriteAllTextAsync(dataPath, serializedData);
                _appLogger.LogInfo($"Data written successfully for type :{typeof(T).Name}");
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"Exception occured in WriteData :{typeof(T).Name}", ex);
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
                    _appLogger.LogInfo($"Data cleared successfully for type :{storeName}");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"Exception occured in ClearData :{storeName}", ex);
            }
        }

        private string GetFilePath(string storeName)
        {
            string dataPath = Path.Combine(_directoryPath, storeName + ".json");
            return dataPath;
        }
    }
}
