using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Gameplay.Models;
using UnityEngine;

namespace Scripts.Gameplay.Controllers
{
    public class DownloadMetaDataFromFirebase
    {
        private const string AndroidPlatform = "Android";
        private const string IosPlatform = "IOS";
        private const string MainJson = "MainJson";

        public async Task<List<AssetBundle>> GetAllUrls(IDisposableObject disposable, string Url)
        {
            string platform = null;
#if UNITY_ANDROID || UNITY_EDITOR
            platform = AndroidPlatform;
#endif
#if UNITY_IOS
            platform = IosPlatform;
#endif
            var urlWithJsonExtension = Url + ".json";
            var master = await App.Services.WebLoader
                                .LoadData<MasterSlaveUrl>(disposable, OnError, urlWithJsonExtension);

            if (!IDisposableObject.IsValid(disposable))
            {
                return null;
            }
            List<AssetBundle> result = new List<AssetBundle>(master.Urls.Count);
            foreach (var item in master.Urls)
            {
                var bundle = await App.Services.WebLoader
                                        .LoadAndGetAssetBundle(disposable, OnError, item.Url);
                if (!IDisposableObject.IsValid(disposable))
                {
                    return null;
                }
                if (bundle != null && bundle.assetbundle != null)
                {
                    result.Add(bundle.assetbundle);
                }
            }
            return result;
        }

        private void OnError(string message)
        {
            Debug.LogError(nameof(DownloadMetaDataFromFirebase) + " " + message);
        }
    }
}