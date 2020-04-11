using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace KAUGamesLviv.Services.Bundles
{
    public class BundleManagerInstance : BundleScriptable<BundleManagerInstance>
    {

        #region Your Custom Implementation of Getting real Url for a given bundlename
        /// <summary>
        /// Works without any of the Firebase plugins (Pure Web requests) 
        /// Customizaed for every one bundle name in separate
        /// </summary>
        public override async System.Threading.Tasks.Task GetUrlCustom(string bundleName, UrlAsStringReference urlToSet, System.Object  options)
        {
            var myOptions = (BundleOptions) options;
            string language = myOptions.LoadLanguage;
            // if (PlayerPrefs.HasKey(BundlesManager.LANGUAGE_FROM_BUNDLE))
            // {
            //     language = BundlesManager.LANGUAGE_FROM_BUNDLE;
            // }

            string someUrl = null;

            if (myOptions.LoadBigdataObject)
            {
                // append .json at the end for Rest API Firebase (Get request,only read)
                string keyNew = "Languages/" + platformToUse + ".json";

                Log(" Key To Get Link From : " + keyNew);
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    urlToSet.url = test.getEmptyURL;
                    return;
                }

                using (var www = UnityWebRequest.Get(DatabaseURL + keyNew)) // (baseUrl + keyNew)
                {
                    await www.SendWebRequest();
                    bool Errors = www.isNetworkError || www.isHttpError;

                    if (!Errors)
                    {
                        someUrl = (string)www.downloadHandler.text;
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
                    urlToSet.url = test.getEmptyURL;
                   return;
                }

                using (var www = UnityWebRequest.Get(DatabaseURL + keyNew)) // (baseUrl + keyNew)
                {
                    await www.SendWebRequest();
                    bool Errors = www.isNetworkError || www.isHttpError;

                    if (!Errors)
                    {
                        someUrl = (string)www.downloadHandler.text;
                        Log("New Url : " + someUrl);
                        urlToSet.url = someUrl.Substring(1, someUrl.Length - 2);
                    }
                    else
                    {
                        Log("Error while Downloading : " + www.error);
                        Log(www.responseCode);
                    }

                }


                // yield return null;
                // if (loadJapanese)
                // {

                //     urlToSet.url = "https://studentutu.github.io/AssetBundles/" + platformToUse + "/" + language + "/" + bundleName;
                // }
                // else
                // {
                //     urlToSet.url = "https://studentutu.github.io/AssetBundles/" + platformToUse + "/" + "en" + "/" + bundleName;

                // }
            }
        }


        /// <summary>
        /// DO NOT CALL base method!
        /// </summary>
        protected override string BundleNameToCacheName(string bundleName, System.Object options)
        {

            var myOptions = (BundleOptions) options;
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
}
