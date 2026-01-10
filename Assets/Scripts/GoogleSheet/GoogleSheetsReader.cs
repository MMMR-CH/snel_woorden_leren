using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using UnityEngine;
using System.Threading.Tasks;

namespace SWL
{
    public static class GoogleSheetsReader
    {
        static SheetsService service;

        static GoogleSheetsReader()
        {
            service = new SheetsService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKeyProvider.GoogleSheetsKey,
                ApplicationName = "Snel Woorden Leren",
            });
        }

        public async static Task<List<List<string>>> ReadSheetData(string spreadsheetId, string range)
        {
            try
            {
                Debug.Log($"Attempting to read sheet: {spreadsheetId}, range: {range}");

                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = await request.ExecuteAsync();

                if (response == null)
                {
                    Debug.LogError("Response is null");
                    return new List<List<string>>();
                }

                var values = response.Values;
                Debug.Log($"Response received. Values count: {(values?.Count ?? 0)}");

                if (values == null || values.Count == 0)
                {
                    Debug.LogWarning("No data found in the sheet. Please check if:");
                    Debug.LogWarning("1. The sheet is publicly accessible");
                    Debug.LogWarning("2. The range is correct");
                    Debug.LogWarning("3. The sheet contains data");
                    return new List<List<string>>();
                }

                var result = new List<List<string>>();
                foreach (var row in values)
                {
                    var rowData = new List<string>();
                    foreach (var cell in row)
                    {
                        rowData.Add(cell.ToString());
                    }
                    result.Add(rowData);
                }

                Debug.Log($"Successfully read {result.Count} rows of data");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading sheet data: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
                return new List<List<string>>();
            }
        }

        public static async Task<List<T>> ReadSheetData<T>(string spreadsheetId, string pageName) where T : new()
        {
            try
            {
                Debug.Log($"Starting deserialization of sheet {pageName} into type {typeof(T).Name}");
                var rawData = await ReadSheetData(spreadsheetId, $"{pageName}!A:Z");
                Debug.Log($"Retrieved {rawData.Count} rows of raw data");

                if (rawData.Count < 2) // Need at least header row and one data row
                {
                    Debug.LogWarning("Not enough data to deserialize - need at least header row and one data row");
                    return new List<T>();
                }

                // Get header row (first row)
                var headers = rawData[0];
                Debug.Log($"Headers found: {string.Join(", ", headers)}");
                var result = new List<T>();

                // Get all properties of type T
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Debug.Log($"Found {properties.Length} properties in type {typeof(T).Name}");

                // Process each data row (skip header row)
                for (int i = 1; i < rawData.Count; i++)
                {
                    var row = rawData[i];
                    Debug.Log($"Processing row {i}: {string.Join(", ", row)}");
                    var item = new T();

                    // Map each column to the corresponding property
                    for (int j = 0; j < Math.Min(headers.Count, row.Count); j++)
                    {
                        var header = headers[j].ToLower();
                        var property = properties.FirstOrDefault(p =>
                            p.Name.ToLower() == header ||
                            p.GetCustomAttribute<GoogleSheetColumnAttribute>()?.ColumnName?.ToLower() == header);

                        if (property != null)
                        {
                            try
                            {
                                var value = row[j];
                                if (!string.IsNullOrEmpty(value))
                                {
                                    Debug.Log($"Setting property {property.Name} with value: {value}");
                                    // Convert the string value to the property type
                                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                                    property.SetValue(item, convertedValue);
                                }
                                else
                                {
                                    Debug.Log($"Skipping empty value for property {property.Name}");
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning($"Failed to set property {property.Name} with value {row[j]}: {e.Message}");
                            }
                        }
                        else
                        {
                            Debug.Log($"No matching property found for header: {header}");
                        }
                    }

                    result.Add(item);
                }

                Debug.Log($"Successfully deserialized {result.Count} items of type {typeof(T).Name}");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deserializing sheet data: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
                return new List<T>();
            }
        }

        public static async Task<List<T>> ReadRangeData<T>(string spreadsheetId, string range) where T : new()
        {
            try
            {
                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = await request.ExecuteAsync();

                Debug.Log($"Response received for range {range}. Values count: {(response.Values?.Count ?? 0)} response: {response}");

                if (response.Values == null || response.Values.Count == 0)
                {
                    Debug.LogWarning($"No data found in range: {range}");
                    return new List<T>();
                }

                // Deserialize the response into a list of T
                return DeserializeSheetData<T>(response.Values);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading range data: {e.Message}");
                Debug.LogError($"Stack trace: {e.StackTrace}");
                return new List<T>();
            }
        }

        private static List<T> DeserializeSheetData<T>(IList<IList<object>> values) where T : new()
        {
            var result = new List<T>();

            // Get the header row
            var headers = values[0];
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Process each data row
            for (int i = 1; i < values.Count; i++)
            {
                var row = values[i];
                var item = new T();

                for (int j = 0; j < Math.Min(headers.Count, row.Count); j++)
                {
                    var header = headers[j].ToString().ToLower();
                    var property = properties.FirstOrDefault(p =>
                        p.Name.ToLower() == header ||
                        p.GetCustomAttribute<GoogleSheetColumnAttribute>()?.ColumnName?.ToLower() == header);

                    if (property != null)
                    {
                        try
                        {
                            var value = row[j]?.ToString();
                            if (!string.IsNullOrEmpty(value))
                            {
                                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                                property.SetValue(item, convertedValue);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to set property {property.Name} with value {row[j]}: {e.Message}");
                        }
                    }
                }

                result.Add(item);
            }

            return result;            
        }
    }
    

    [AttributeUsage(AttributeTargets.Property)]
    public class GoogleSheetColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public GoogleSheetColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
