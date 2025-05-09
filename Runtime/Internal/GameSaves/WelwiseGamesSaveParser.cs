using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace WelwiseGamesSDK.Internal.GameSaves
{
    internal sealed class WelwiseGamesSaveParser : ISaveParser
    {
        public void DeserializeJsonToContainer(string json, DataContainer container)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<GetGameDataResponseModel>(json);
                if (response?.PlayerGameData == null)
                {
                    Debug.LogError($"[{nameof(WelwiseGamesSaveParser)}] Can't parse game data");
                    return;
                }

                container.Clear();
                container.PlayerName = response.PlayerName ?? container.PlayerName;
                foreach (var data in response.PlayerGameData)
                {
                    if (!string.IsNullOrEmpty(data.Identifier)
                        && !string.IsNullOrEmpty(data.Value))
                    {
                        container.ParseKeyValue(data.Identifier, data.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public string SerializeContainerToJson(DataContainer container)
        {
            try
            {
                var response = new GetGameDataResponseModel
                {
                    PlayerName = container.PlayerName,
                    PlayerGameData = CreateGameDataArray(container)
                };

                return JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "{}";
            }
        }
        
        private static GameDataModel[] CreateGameDataArray(DataContainer container)
        {
            var result = container.Booleans.Select(kvp => new GameDataModel { Identifier = kvp.Key, Value = $"b{kvp.Value}" }).ToList();
            result.AddRange(container.Ints
                .Select(kvp => new GameDataModel { Identifier = kvp.Key, Value = $"i{kvp.Value}" }));
            result.AddRange(container.Floats
                .Select(kvp => new GameDataModel { Identifier = kvp.Key, Value = $"f{kvp.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}" }));
            result.AddRange(container.Strings
                .Select(kvp => new GameDataModel { Identifier = kvp.Key, Value = $"s{kvp.Value}" }));
            return result.ToArray();
        }

        
        private class GetGameDataResponseModel
        {
            public string PlayerName { get; set; }
            public GameDataModel[] PlayerGameData { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class GameDataModel
        {
            public string Identifier { get; set; }
            public string Value { get; set; }
            public string[] Values { get; set; }
        }
    }
}