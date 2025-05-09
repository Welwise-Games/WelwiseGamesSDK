using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace WelwiseGamesSDK.Internal.GameSaves
{
    internal sealed class YandexGamesSaveParser : ISaveParser
    {
        public void DeserializeJsonToContainer(string json, DataContainer container)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (dict == null)
                {
                    Debug.LogError($"[{nameof(YandexGamesSaveParser)}] JSON parsing failed");
                    return;
                }
                container.Clear();

                foreach (var kvp in dict)
                {
                    if (kvp.Key.Equals("playerName", StringComparison.OrdinalIgnoreCase))
                    {
                        container.PlayerName = kvp.Value?.ToString();
                        continue;
                    }

                    if (kvp.Value is string valueStr)
                    {
                        container.ParseKeyValue(kvp.Key, valueStr);
                    }
                    else
                    {
                        Debug.LogWarning($"[{nameof(YandexGamesSaveParser)}] Value for key {kvp.Key} is not a string: {kvp.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(YandexGamesSaveParser)}] JSON parsing failed: {e.Message}");
            }
        }

        public string SerializeContainerToJson(DataContainer container)
        {
            try
            {
                var dict = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(container.PlayerName))
                    dict["playerName"] = container.PlayerName;

                AddValuesToDictionary(container, dict);
        
                return JsonConvert.SerializeObject(dict);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "{}";
            }
        }
        private static void AddValuesToDictionary(DataContainer container, Dictionary<string, object> dict)
        {
            foreach (var kvp in container.Booleans)
                dict[kvp.Key] = $"b{kvp.Value}";

            foreach (var kvp in container.Ints)
                dict[kvp.Key] = $"i{kvp.Value}";

            foreach (var kvp in container.Floats)
                dict[kvp.Key] = $"f{kvp.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            foreach (var kvp in container.Strings)
                dict[kvp.Key] = $"s{kvp.Value}";
        }
        
    }
}