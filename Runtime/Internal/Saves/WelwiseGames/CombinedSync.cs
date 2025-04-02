using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Responses;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal class CombinedSync
    {
        private UnityWebRequest _request;
        private bool _initialSyncRequested;
        private bool _initialSyncResponse;
        private readonly WebSender _webSender;
        private string _url;
        private List<Action<UnityWebRequest>> _callbacks = new List<Action<UnityWebRequest>>();
        private SaveGameDataRequest _saveGameDataRequest;
        private SaveMetaverseDataRequest _saveMetaverseDataRequest;
        private bool _saving;
        private string _lastName;
        

        public CombinedSync(WebSender webSender)
        {
            _webSender = webSender;
        }

        public void SetURL(string url)
        {
            _url = url;
        }

        public void InitialSync(Action<UnityWebRequest> callback)
        {
            if (!_initialSyncRequested)
            {
                _webSender.GetRequest(
                    _url,
                    InitialSyncCallback);
            }
            
            _initialSyncRequested = true;
            
            if (!_initialSyncResponse) _callbacks.Add(callback);
            else callback?.Invoke(_request);
        }

        private void InitialSyncCallback(UnityWebRequest r)
        {
            foreach (var callback in _callbacks)
            {
                callback?.Invoke(r);
            }
            _request = r;
            var data = JsonConvert.DeserializeObject<GetMetaverseWithGameDataResponse>(_request.downloadHandler.text);
            _lastName = data.PlayerName;
            _saveMetaverseDataRequest = new SaveMetaverseDataRequest()
            {
                PlayerName = _lastName,
                PlayerMetaverseData = data.PlayerMetaverseData
            };
            _saveGameDataRequest = new SaveGameDataRequest()
            {
                PlayerName = _lastName,
                PlayerGameData = data.PlayerGameData
            };
            _initialSyncResponse = true;
        }

        public void SetMetaverseDataRequest(SaveMetaverseDataRequest saveMetaverseDataRequest)
        {
            _saveMetaverseDataRequest = saveMetaverseDataRequest;
            Sync();
        }

        public void SetSaveGameDataRequest(SaveGameDataRequest saveGameDataRequest)
        {
            _saveGameDataRequest = saveGameDataRequest;
            Sync();
        }

        private void Sync()
        {
            if (_saving) return;
            if (_saveGameDataRequest == null) return;
            if (_saveMetaverseDataRequest == null) return;
            
            _saving = true;
            var combinedData = new SaveMetaverseWithGameDataRequest();
            if (_saveGameDataRequest.PlayerName != null && _saveGameDataRequest.PlayerName != _lastName)
            {
                _lastName = _saveGameDataRequest.PlayerName;
            }
            if (_lastName != null && _saveMetaverseDataRequest.PlayerName != _lastName)
            {
                _lastName = _saveMetaverseDataRequest.PlayerName;
            }
            combinedData.PlayerName = _lastName;
            combinedData.PlayerGameData = _saveGameDataRequest.PlayerGameData;
            combinedData.PlayerMetaverseData = _saveMetaverseDataRequest.PlayerMetaverseData;
            
            var json = JsonConvert.SerializeObject(combinedData);
            
            _webSender.PutRequest(
                _url,
                json,
                (_) =>
                {
                    _saving = false;
                });
        }
    }
}