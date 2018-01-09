// Copyright (c) 2016 Ez Entertainment SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez
{
    public static class Http
    {
        private static GameObject _httpObject = null;
        private static MonoBehaviour _httpBehaviour = null;
        private static MonoBehaviour HttpBehaviour { get { if (_httpBehaviour == null) { _httpObject = new GameObject("HttpObject", typeof(MonoBehaviour)); _httpBehaviour = _httpObject.GetComponent<MonoBehaviour>(); } return _httpBehaviour; } }
        public delegate void HttpCallback(string result);
        private static void HttpCallbackHandler(WWW w, HttpCallback callback)
        {
            if (w.error != null)
            {
#if UNITY_EDITOR
                Debug.Log("Http Error: " + w.error);
#endif
                return;
            }

            if (callback != null) callback(w.text);
        }
        public static void GetRequest(string request, HttpCallback callback = null) { HttpBehaviour.StartCoroutine(SendAsyncGetRequest(request, callback)); }
        private static IEnumerator SendAsyncGetRequest(string request, HttpCallback callback) { WWW w = new WWW(request); yield return w; HttpCallbackHandler(w, callback); }
        public static void PostRequest(string request, byte[] data, Dictionary<string, string> headers, HttpCallback callback = null) { HttpBehaviour.StartCoroutine(SendAsyncPostRequest(request, data, headers, callback)); }
        private static IEnumerator SendAsyncPostRequest(string request, byte[] data, Dictionary<string, string> headers, HttpCallback callback) { WWW w = new WWW(request, data, headers); yield return w; HttpCallbackHandler(w, callback); }
    }
}
