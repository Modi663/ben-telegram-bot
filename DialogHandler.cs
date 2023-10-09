using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Telegram.Bots.Types;
using File = System.IO.File;

namespace BenTelegramBot
{
    internal class DialogHandler
    {
        private static string path = "../../../dialoges.json";
        public void AddDialog(long key, bool value)
        {
            List<KeyValuePair<long, bool>> keyValuePairs = new List<KeyValuePair<long, bool>> { new (key, value) };
            string updatedJsonString = JsonSerializer.Serialize(keyValuePairs);
            File.WriteAllText(path, updatedJsonString);
            Console.WriteLine($"Диалог {key} добавлен");
        }

        public async Task RemoveDialog(long key)
        {
            string jsonString = File.ReadAllText(path);

            if (!string.IsNullOrEmpty(jsonString))
            {
                List<KeyValuePair<long, bool>>? keyValuePairs = JsonSerializer.Deserialize<List<KeyValuePair<long, bool>>>(jsonString);

                bool foundKey = false;
                for (int i = 0; i < keyValuePairs?.Count; i++)
                {
                    if (keyValuePairs[i].Key == key)
                    {
                        foundKey = true;
                        keyValuePairs[i] = new KeyValuePair<long, bool>(key, false);
                        break;
                    }
                }
                if (foundKey)
                {
                    string updatedJsonString = JsonSerializer.Serialize(keyValuePairs);
                    File.WriteAllText(path, updatedJsonString);
                    Console.WriteLine($"Диалог {key} удален");
                }else { Console.WriteLine($"Ключ {key} не найден."); }
            }else { Console.WriteLine("Файл пуст или не найден."); }
        }

        public bool inDialog(long key)
        {
            string jsonString = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(jsonString))
            {
                List<KeyValuePair<long, bool>>? keyValuePairs = JsonSerializer.Deserialize<List<KeyValuePair<long, bool>>>(jsonString);
                bool foundValue = false;
                if (keyValuePairs != null)
                    foreach (KeyValuePair<long, bool> kvp in keyValuePairs)
                    {
                        if (kvp.Key == key)
                        {
                            foundValue = kvp.Value;
                            break;
                        }
                    }

                return foundValue;
            }else { return false; }
        }
    }
}
