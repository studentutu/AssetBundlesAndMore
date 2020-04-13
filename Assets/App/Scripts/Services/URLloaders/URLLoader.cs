using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Scripts.Services
{
    [Serializable]
    public class URLLoader : IServices, Entry.ISetUrl
    {
        public class WrapperDownload
        {
            public DownloadHandler handler;
            public AssetBundle assetbundle;
        }

        [SerializeField] private string mainUrl = "";
        public string MainUrl => mainUrl;

        void IServices.Init()
        {

        }

        void Entry.ISetUrl.SetUrl(string newUrl)
        {
            mainUrl = newUrl;
        }

        /// <summary>
        /// Load all games data
        /// </summary>
        /// <param name="callback">success callback</param>
        /// <param name="error">error callback</param>
        public async Task<T> LoadData<T>(
            IDisposableObject refToCheck,
            Action<string> error,
            string url)
        {
            using (var req = UnityWebRequest.Get(url))
            {
#pragma warning disable
                req.SendWebRequest();
#pragma warning restore
                while (!req.isDone && !req.isHttpError && !req.isNetworkError && IDisposableObject.IsValid(refToCheck))
                {
                    await Task.Yield(); // 0.005s
                }


                if (!IDisposableObject.IsValid(refToCheck) || req.isHttpError || req.isNetworkError)
                {
                    error.Invoke(req.error);
                    return default;
                }
                else
                {
                    return App.Services.JsonConverter.FromJson<T>(req.downloadHandler.text);
                }
            }
        }

        /// <summary>
        /// Load all games data
        /// </summary>
        /// <param name="callback">success callback</param>
        /// <param name="error">error callback</param>
        public async Task<string> LoadData(
            IDisposableObject refToCheck,
            Action<string> error,
            string url)
        {
            using (var req = UnityWebRequest.Get(url))
            {
#pragma warning disable
                req.SendWebRequest();
#pragma warning restore
                while (!req.isDone && !req.isHttpError && !req.isNetworkError && IDisposableObject.IsValid(refToCheck))
                {
                    await Task.Yield(); // 0.005s
                }


                if (!IDisposableObject.IsValid(refToCheck) || req.isHttpError || req.isNetworkError)
                {
                    error.Invoke(req.error);
                    return null;
                }
                else
                {
                    return req.downloadHandler.text;
                }
            }
        }

        /// <summary>
        /// Load all games data
        /// </summary>
        /// <param name="callback">success callback</param>
        /// <param name="error">error callback</param>
        public async Task<WrapperDownload> LoadAndGetDownloadHandler(
            IDisposableObject refToCheck,
            Action<string> error,
            string url)
        {
            using (var req = UnityWebRequest.Get(url))
            {
#pragma warning disable
                req.SendWebRequest();
#pragma warning restore
                while (!req.isDone && !req.isHttpError &&
                        !req.isNetworkError && IDisposableObject.IsValid(refToCheck))
                {
                    await Task.Yield(); // 0.005s
                }


                if (!IDisposableObject.IsValid(refToCheck) || req.isHttpError || req.isNetworkError)
                {
                    error.Invoke(req.error);
                    return null;
                }
                else
                {
                    var result = new WrapperDownload
                    {
                        handler = req.downloadHandler
                    };
                    return result;
                }
            }
        }

        /// <summary>
        /// Load all games data. Caches it inside the device cached folder
        /// </summary>
        /// <param name="callback">success callback</param>
        /// <param name="error">error callback</param>
        public async Task<WrapperDownload> LoadAndGetAssetBundle(
            IDisposableObject refToCheck,
            Action<string> error,
            string url)
        {
            using (var req = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
#pragma warning disable
                req.SendWebRequest();
#pragma warning restore
                while (!req.isDone && !req.isHttpError &&
                        !req.isNetworkError && IDisposableObject.IsValid(refToCheck))
                {
                    await Task.Yield(); // 0.005s
                }


                if (!IDisposableObject.IsValid(refToCheck) || req.isHttpError || req.isNetworkError)
                {
                    error.Invoke(req.error);
                    return null;
                }
                else
                {
                    var result = new WrapperDownload
                    {
                        assetbundle = DownloadHandlerAssetBundle.GetContent(req)
                    };
                    return result;
                }
            }
        }
    }
}
