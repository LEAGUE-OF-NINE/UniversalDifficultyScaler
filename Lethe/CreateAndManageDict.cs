using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace MyPlugin
{
    internal class CreateAndManageDict
    {
        public static async Task<Dictionary<string, float>> LoadOrCreateDictionaryAsync(string filePath, IEnumerable<string> keysToInitialize)
        {
            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<Dictionary<string, float>>(json)
                       ?? new Dictionary<string, float>();
            }
            else
            {
                var newDict = new Dictionary<string, float>();
                foreach (var key in keysToInitialize)
                {
                    newDict[key] = 0;
                }
                
                string json = JsonSerializer.Serialize(newDict, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);

                return newDict;
            }
        }

    }
}
