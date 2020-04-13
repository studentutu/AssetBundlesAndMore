using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Services.Bundles;
using System.Threading.Tasks;
using Scripts;
using Scripts.Gameplay.Controllers;

// [CreateAssetMenu(fileName = "Container", menuName = "Bundles/Example", order = 0)]
public class BundleManagerAppExample : BundleService<BundleManagerAppExample>
{
    // Public%2FAndroid%2Fnewmarkers?alt=media&
    [SerializeField] private string CachedNamesRexExp = "Public%2F{0}%2F{1}";

    #region Your Custom Implementation of Getting real Url for a given bundlename
    protected override Task GetUrlAsTask(string bundleName, UrlAsStringReference urlToSet, object options)
    {
        return GetUrl(bundleName, urlToSet, options);
    }

    /// <summary>
    /// Works without any of the Firebase plugins (Pure Web requests) 
    /// Customized for every one bundle name in separate
    /// </summary>
    private async Task GetUrl(string bundleName, UrlAsStringReference urlToSet, System.Object options)
    {
        var master = App.GetController<DownloadMetaDataFromFirebase>();
        if (master.actualDataBase == null)
        {
            await master.GetAllUrls(new IDisposableObject(), master.MainUrl);
        }
        CheckIfNull(master, bundleName, urlToSet);
    }

    private void CheckIfNull(DownloadMetaDataFromFirebase handler, string bundleName, UrlAsStringReference urlToSet)
    {
        // append .json at the end for Rest API Firebase (Get request,only read)
        string urlDownload = null;
        for (int i = 0; i < handler.actualDataBase.Urls.Count; i++)
        {
            if (handler.actualDataBase.Urls[i].Name.ToLower().Equals(bundleName))
            {
                urlDownload = handler.actualDataBase.Urls[i].Url;
                break;
            }
        }
        urlToSet.url = urlDownload;
    }

    /// <summary>
    /// DO NOT CALL base method!
    /// </summary>
    protected override string BundleNameToCacheName(string bundleName, System.Object options)
    {
        // Example of implementing custom Mapping
        var pathInStorage = string.Format(CachedNamesRexExp, platformToUse, bundleName);
        return pathInStorage;
    }

    #endregion // Your Custom Implementation of Getting real Url for a given bundlename
}