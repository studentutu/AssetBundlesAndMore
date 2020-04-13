using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Scripts.Gameplay.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Scripts.Services
{
    public abstract class DownloadController<T> : IServices
        where T : class
    {
        private enum DataType
        {
            NotInitialized = 0,
            Custom,
            AssetBundle
        }

        private Dictionary<string, T> allManaged = null;
        private Task<bool> startedTask = null;
        private IDisposableObject disposableObject = null;
        private DataType dataType = DataType.NotInitialized;

        void IServices.Init()
        {
            App.ApplicationQuittingEvent -= OnQuit;
            App.ApplicationQuittingEvent += OnQuit;
            startedTask = LoadingAllContent();
        }

        private void OnQuit()
        {
            disposableObject?.Dispose();
            startedTask = null;
        }

        protected abstract bool IsUnityObject { get; }
        protected abstract T ParseFromHandler(DownloadHandler handler);


        public async Task<bool> LoadingAllContent()
        {
            if (startedTask != null)
            {
                var result = await startedTask;
                startedTask = null;
                return result;
            }
            else
            {
                disposableObject = new IDisposableObject();
                var listOfItems = await App.Services.WebLoader.LoadData(disposableObject,
                (msg) => { Debug.LogError(msg); },
                App.Services.WebLoader.MainUrl
                );

                if (string.IsNullOrEmpty(listOfItems))
                {
                    Debug.LogWarning("string is null");
                    return false;
                }
                if (!IDisposableObject.IsValid(disposableObject))
                {
                    return false;
                }

                var master = App.Services.JsonConverter.FromJson<MasterSlaveUrl>(listOfItems);
                if (allManaged == null)
                {
                    allManaged = new Dictionary<string, T>();
                }
                else
                {
                    if (IsUnityObject)
                    {
                        foreach (var key in allManaged.Keys)
                        {
                            if (allManaged[key] != null)
                            {
                                Object.Destroy(allManaged[key] as UnityEngine.Object);
                            }
                        }
                    }
                }
                allManaged.Clear();


                if (dataType == DataType.NotInitialized)
                {
                    var typeT = typeof(T);
                    dataType = DataType.Custom;

                    bool isAssetBundle = typeT == typeof(AssetBundle);
                    if (isAssetBundle)
                    {
                        dataType = DataType.AssetBundle;
                    }
                }

                bool shouldBeParsed = dataType == DataType.Custom;
                for (int i = 0; i < master.Urls.Count; i++)
                {

                    URLLoader.WrapperDownload downloadHandler = null;
                    switch (dataType)
                    {
                        case DataType.Custom:
                            downloadHandler = await App.Services.WebLoader.LoadAndGetDownloadHandler(disposableObject,
                                (msg) => { Debug.LogError(msg); },
                                master.Urls[i].Url);
                            break;
                        case DataType.AssetBundle:
                            downloadHandler = await App.Services.WebLoader.LoadAndGetAssetBundle(disposableObject,
                                (msg) => { Debug.LogError(msg); },
                                master.Urls[i].Url);
                            break;
                    }

                    if (!IDisposableObject.IsValid(disposableObject))
                    {
                        return false;
                    }
                    if (downloadHandler.assetbundle == null)
                    {
                        var parser = ParseFromHandler(downloadHandler.handler);
                        if (parser == null)
                        {
                            Debug.LogWarning($" could not parse {master.Urls[i].Name}");
                            continue;
                        }
                        allManaged.Add(master.Urls[i].Name, parser);
                    }
                    else
                    {
                        allManaged.Add(master.Urls[i].Name, downloadHandler.assetbundle as T);
                    }
                }
                return true;
            }
        }
    }
}