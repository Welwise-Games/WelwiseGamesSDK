using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace WelwiseGamesSDK.Internal
{
    internal class WebSender : MonoBehaviour
    {
        private const string ApiKeyHeader = "X-API-Key";
        private string _apiKey;

        public void Initialize(string apiKey)
        {
            _apiKey = apiKey;
        }

        public static WebSender Create()
        {
            var go = new GameObject("WelwiseGamesSDK.WebSender");
            DontDestroyOnLoad(go);
            return go.AddComponent<WebSender>();
        }
        
        #region Routines
        private IEnumerator GetRequestRoutine(
            string url, 
            Action<UnityWebRequest> callback)
        {
            using var request = UnityWebRequest.Get(url);
            request.SetRequestHeader(ApiKeyHeader, _apiKey);
            yield return request.SendWebRequest();
            
            callback?.Invoke(request);
        }

        IEnumerator PostRequestRoutine(
            string url, 
            string jsonData, 
            Action<UnityWebRequest> callback,
            string headerName = "Content-Type", 
            string headerValue = "application/json")
        {
            using var request = new UnityWebRequest(url, "POST");
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader(ApiKeyHeader, _apiKey);
            request.SetRequestHeader(headerName, headerValue);

            yield return request.SendWebRequest();
            
            callback?.Invoke(request);
        }


        IEnumerator PostFormDataRoutine(
            string url, 
            WWWForm form, 
            Action<UnityWebRequest> callback)
        {
            using UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader(ApiKeyHeader, _apiKey);
            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }

        IEnumerator DeleteRequestRoutine(
            string url, 
            Action<UnityWebRequest> callback)
        {
            using var request = UnityWebRequest.Delete(url);
            request.SetRequestHeader(ApiKeyHeader, _apiKey);
            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }

        IEnumerator PutRequestRoutine(
            string url, 
            string jsonData, 
            Action<UnityWebRequest> callback, 
            string headerName = "Content-Type", 
            string headerValue = "application/json")
        {
            using var request = new UnityWebRequest(url, "PUT");
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader(ApiKeyHeader, _apiKey);
            request.SetRequestHeader(headerName, headerValue);

            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }
        #endregion

        
        #region Methods
        public void GetRequest(
            string url, 
            Action<UnityWebRequest> callback) =>
            StartCoroutine(GetRequestRoutine(url, callback));

        public void PostRequest(
            string url, 
            string jsonData, 
            Action<UnityWebRequest> callback,
            string headerName = "Content-Type", string headerValue = "application/json") =>
            StartCoroutine(PostRequestRoutine(url, jsonData, callback, headerName, headerValue));
        
        public void PostFormData(
            string url, 
            WWWForm form, 
            Action<UnityWebRequest> callback) =>
            StartCoroutine(PostFormDataRoutine(url, form, callback));

        public void DeleteRequest(
            string url, 
            Action<UnityWebRequest> callback) =>
            StartCoroutine(DeleteRequestRoutine(url, callback));

        public void PutRequest(
            string url, 
            string jsonData, 
            Action<UnityWebRequest> callback, 
            string headerName = "Content-Type", 
            string headerValue = "application/json") => 
            StartCoroutine(PutRequestRoutine(url, jsonData, callback, headerName, headerValue));
        #endregion
    }
}