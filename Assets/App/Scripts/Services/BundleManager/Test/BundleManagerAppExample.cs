using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Services.Bundles;
using System.Threading.Tasks;

// [CreateAssetMenu(fileName = "Container", menuName = "Bundles/Example", order = 0)]
public class BundleManagerAppExample : BundleService<BundleManagerAppExample>
{

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
        var myOptions = options as TestBundleService.BundleOptions;
        string language = myOptions.LoadLanguage;
        // if (PlayerPrefs.HasKey(BundlesManager.LANGUAGE_FROM_BUNDLE))
        // {
        //     language = BundlesManager.LANGUAGE_FROM_BUNDLE;
        // }

        if (myOptions.LoadBigdataObject)
        {
            // append .json at the end for Rest API Firebase (Get request,only read)
            string keyNew = platformToUse + ".json";

            Log(" Key To Get Link From : " + keyNew);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                urlToSet.url = TestBundleService.getEmptyURL;
                return;
            }

            using (var www = UnityWebRequest.Get(DatabaseURL + keyNew)) // (baseUrl + keyNew)
            {
                await www.SendWebRequest();
                bool Errors = www.isNetworkError || www.isHttpError;

                if (!Errors)
                {
                    string someUrl = (string)www.downloadHandler.text;
                    Log("New Url : " + someUrl);
                    urlToSet.url = someUrl.Substring(1, someUrl.Length - 2);
                }
                else
                {
                    Log("Error while Downloading : " + www.error);
                    Log(www.responseCode);
                }
            }
        }
        else
        {

            // append .json at the end for Rest API Firebase (Get request,only read)
            string keyNew = "Updater/" + platformToUse + "/";
            if (!string.IsNullOrEmpty(language))
            {
                keyNew += language + "/";
            }
            keyNew += bundleName + ".json";

            Log(" Key To Get Link From : " + keyNew);
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                urlToSet.url = TestBundleService.getEmptyURL;
                return;
            }

            using (var www = UnityWebRequest.Get(DatabaseURL + keyNew)) // (baseUrl + keyNew)
            {
                await www.SendWebRequest();
                bool Errors = www.isNetworkError || www.isHttpError;

                if (!Errors)
                {
                    string someUrl = (string)www.downloadHandler.text;
                    Log("New Url : " + someUrl);
                    urlToSet.url = someUrl.Substring(1, someUrl.Length - 2);
                }
                else
                {
                    Log("Error while Downloading : " + www.error);
                    Log(www.responseCode);
                }
            }
        }
    }


    /// <summary>
    /// DO NOT CALL base method!
    /// </summary>
    protected override string BundleNameToCacheName(string bundleName, System.Object options)
    {
        var myOptions = options as TestBundleService.BundleOptions;
        if (myOptions.LoadBigdataObject)
        {
            return "LanguageListing%2F" + platformToUse + "%2F" + bundleName;
        }
        // Example of implementing custom Mapping
        var prefix = myOptions.LoadLanguage;

        // if (PlayerPrefs.HasKey(BundlesManager.LANGUAGE_FROM_BUNDLE))
        // {
        //     prefix = BundlesManager.LANGUAGE_FROM_BUNDLE;
        // }
        string pathInStorage = "Bundles%2F" + platformToUse + "%2F" + prefix + "%2F" + bundleName;

        return pathInStorage;
    }

    #endregion // Your Custom Implementation of Getting real Url for a given bundlename
}