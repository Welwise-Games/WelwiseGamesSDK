using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Data;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Responses;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Responses;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves.WelwiseGames
{
    internal sealed class GamePlatformSaves : PlatformSaves
    {
        public GamePlatformSaves(
            WebSender webSender, 
            float syncDelay, 
            IEnvironment environment,
            bool useMetaverse,
            string metaverseId,
            string gameId,
            CombinedSync sync)
            : base(
                webSender, 
                syncDelay, 
                environment,
                useMetaverse,
                metaverseId,
                gameId,
                sync)
        {
        }

        protected override void ParseSaveJson(string json)
        {
            if (_useMetaverse)
            {
                ParseCombinedJson(json);
            }
            else
            {
                ParseDefaultJson(json);
            }
        }

        private void ParseCombinedJson(string json)
        {
            var data = JsonConvert.DeserializeObject<GetMetaverseWithGameDataResponse>(json);
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

        private void ParseDefaultJson(string json)
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


        protected override string GetUrl() => $"{BaseApiUrl}/games/{_gameId}/players/{_environment.PlayerId}";


        protected override string CreateSaveJson()
        {
            var request = CreateSaveGameDataRequest();
            return JsonConvert.SerializeObject(request);
        }

        protected override SaveGameDataRequest CreateSaveGameDataRequest()
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
            return request;
        }

        protected override SaveMetaverseDataRequest CreateSaveMetaverseDataRequest() => null;

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