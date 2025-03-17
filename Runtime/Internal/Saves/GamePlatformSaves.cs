using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Data;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Responses;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal sealed class GamePlatformSaves : PlatformSaves
    {
        private string _playerName;
        
        public GamePlatformSaves(WebSender webSender) : base(webSender) {}

        protected override void ParseSaveJson(string json)
        {
            var data = JsonConvert.DeserializeObject<GetGameDataResponse>(json);
            _playerName = data.PlayerName;
            foreach (var gameData in data.PlayerGameData)
            {
                if (!string.IsNullOrEmpty(gameData.Value))
                {
                    ParseSimpleValue(gameData.Identifier, gameData.Value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        
        public override string GetPlayerName() => _playerName;

        public override void SetPlayerName(string name)
        {
            _playerName = name;
            SyncCheck();
        }


        protected override string GetUrl() => $"{WelwiseSDK.BaseUrl}/games/{WelwiseSDK.Settings.GameId}/players/{WelwiseSDK.GetEnvironment().PlayerId.ToString()}";


        protected override string CreateSaveJson()
        {
            var gameData = new List<GameData>();

            foreach (var kvp in _strings)
            {
                gameData.Add(new GameData()
                {
                    Identifier = kvp.Key,
                    Value = $"s{kvp.Value}"
                });
            }

            foreach (var kvp in _floats)
            {
                gameData.Add(new GameData()
                {
                    Identifier = kvp.Key,
                    Value = $"f{kvp.Value.ToString(CultureInfo.InvariantCulture)}"
                });
            }

            foreach (var kvp in _ints)
            {
                gameData.Add(new GameData()
                {
                    Identifier = kvp.Key,
                    Value = $"i{kvp.Value.ToString()}"
                });
            }

            foreach (var kvp in _booleans)
            {
                gameData.Add(new GameData()
                {
                    Identifier = kvp.Key,
                    Value = $"b{kvp.Value.ToString()}"
                });
            }

            var request = new SaveGameDataRequest()
            {
                PlayerName = _playerName,
                PlayerGameData = gameData.ToArray()
            };

            return JsonConvert.SerializeObject(request);
        }
        
        private void ParseSimpleValue(string identifier, string value)
        {
            var type = value[0];
            switch (type)
            {
                case 's': 
                    _strings[identifier] = value.Substring(1);
                    break;
                case 'i':
                    _ints[identifier] = int.Parse(value.Substring(1));
                    break;
                case 'f':
                    _floats[identifier] = float.Parse(value.Substring(1));
                    break;
                case 'b':
                    _booleans[identifier] = bool.Parse(value.Substring(1));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}