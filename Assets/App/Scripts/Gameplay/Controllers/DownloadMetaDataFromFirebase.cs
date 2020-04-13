using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Gameplay.Models;
using UnityEngine;

namespace Scripts.Gameplay.Controllers
{
    public class DownloadMetaDataFromFirebase : IController
    {
        private const string AndroidPlatform = "Android";
        private const string IosPlatform = "IOS";
        private const string MainJson = "MainJson";
        [SerializeField] private string mainUrl = null;
        [HideInInspector] public MasterSlaveUrl actualDataBase = null;

        public string MainUrl => mainUrl;

        public async Task GetAllUrls(IDisposableObject disposable, string Url)
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
                return;
            }
            actualDataBase = master;
        }

        public void Init(IModel model, IView view)
        {

        }

        private void OnError(string message)
        {
            Debug.LogError(nameof(DownloadMetaDataFromFirebase) + " " + message);
        }
    }
}