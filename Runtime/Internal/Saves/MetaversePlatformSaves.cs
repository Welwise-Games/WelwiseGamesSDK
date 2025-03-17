using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Data;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Responses;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal class MetaversePlatformSaves : PlatformSaves
    {
        private string _playerName;

        public MetaversePlatformSaves(WebSender webSender) : base(webSender) {}
        
        protected override void ParseSaveJson(string json)
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

        protected override string GetUrl() => $"{WelwiseSDK.BaseUrl}/metaverses/{WelwiseSDK.Settings.MetaverseId}/players/{WelwiseSDK.GetEnvironment().PlayerId.ToString()}";

        protected override string CreateSaveJson()
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
            return JsonConvert.SerializeObject(request);
        }

        public override string GetPlayerName() => _playerName;

        public override void SetPlayerName(string name)
        {
            _playerName = name;
            SyncCheck();
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