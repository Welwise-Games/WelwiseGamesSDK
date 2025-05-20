using UnityEngine;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal static class DataMarshallingUtil
    {
        public static string Serialize(int value) => $"i{value}";
        public static string Serialize(float value) => $"f{value.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
        public static string Serialize(bool value) => $"b{value}";
        public static string Serialize(string value) => $"s{value}";
        
        internal static void DeserializeToContainer(this DataContainer dataContainer, string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            var prefix = value[0];
            var content = value.Length > 1 ? value[1..] : "";

            switch (prefix)
            {
                case 'b':
                    if (bool.TryParse(content, out var boolVal))
                        dataContainer.Booleans[key] = boolVal;
                    else
                        Debug.LogWarning($"Invalid bool value: {content} for key {key}");
                    break;
                case 'i':
                    if (int.TryParse(content, out var intVal))
                        dataContainer.Ints[key] = intVal;
                    else
                        Debug.LogWarning($"Invalid int value: {content} for key {key}");
                    break;
                case 'f':
                    if (float.TryParse(content, 
                            System.Globalization.NumberStyles.Float, 
                            System.Globalization.CultureInfo.InvariantCulture, 
                            out var floatVal))
                        dataContainer.Floats[key] = floatVal;
                    else
                        Debug.LogWarning($"Invalid float value: {content} for key {key}");
                    break;
                case 's':
                    dataContainer.Strings[key] = content;
                    break;
                default:
                    Debug.LogWarning($"Unknown prefix '{prefix}' for key {key}");
                    break;
            }
        }
        
    }
}