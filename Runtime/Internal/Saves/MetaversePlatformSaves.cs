using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Plastic.Newtonsoft.Json;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Data;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Responses;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal class MetaversePlatformSaves : PlatformSaves
    {
        
        public MetaversePlatformSaves(
            WebSender webSender, 
            float syncDelay, 
            IEnvironment environment,
            bool useMetaverse,
            string metaverseId,
            string gameId,
            CombinedSync combinedSync) 
            : base(
                webSender, 
                syncDelay, 
                environment,
                useMetaverse,
                metaverseId,
                gameId,
                combinedSync)
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
            foreach (var gameData in data.PlayerMetaverseData)
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
            var data = JsonConvert.DeserializeObject<GetMetaverseDataResponse>(json);
            _playerName = data.PlayerName;
            foreach (var gameData in data.PlayerMetaverseData)
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

        protected override string GetUrl() => $"{BaseApiUrl}/metaverses/{_metaverseId}/players/{_environment.PlayerId}";

        protected override string CreateSaveJson()
        {
            var request = CreateSaveMetaverseDataRequest();
            return JsonConvert.SerializeObject(request);
        }

        protected override SaveGameDataRequest CreateSaveGameDataRequest() => null;

        protected override SaveMetaverseDataRequest CreateSaveMetaverseDataRequest()
        {
            var gameData = new List<MetaverseData>();

            foreach (var kvp in _strings)
            {
                gameData.Add(new MetaverseData()
                {
                    Identifier = kvp.Key,
                    Value = $"s{kvp.Value}"
                });
            }

            foreach (var kvp in _floats)
            {
                gameData.Add(new MetaverseData()
                {
                    Identifier = kvp.Key,
                    Value = $"f{kvp.Value.ToString(CultureInfo.InvariantCulture)}"
                });
            }

            foreach (var kvp in _ints)
            {
                gameData.Add(new MetaverseData()
                {
                    Identifier = kvp.Key,
                    Value = $"i{kvp.Value.ToString()}"
                });
            }

            foreach (var kvp in _booleans)
            {
                gameData.Add(new MetaverseData()
                {
                    Identifier = kvp.Key,
                    Value = $"b{kvp.Value.ToString()}"
                });
            }

            
            var request = new SaveMetaverseDataRequest()
            {
                PlayerName = _playerName,
                PlayerMetaverseData = gameData.ToArray()
            };
            
            return request;
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