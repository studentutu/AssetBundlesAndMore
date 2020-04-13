using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Networking;
using System;
using Services.Bundles;

public class TestBundleService : MonoBehaviour
{
    public class BundleOptions
    {
        public string LoadLanguage = "en";
        public bool LoadBigdataObject = false;
    }
    [SerializeField] public BundleManagerAppExample instanceOfBundles;
    public static TestBundleService Instance;

    public static string getEmptyURL
    {
        get
        {
            return Instance.urlToloadFrom;
        }
    }
    [SerializeField] public string urlToloadFrom = "";


    public static ulong bytsFull = 0;

    public void LoadEnglish()
    {
        bundleName = "lvl3";
        LoadBigData = false;
        nowLoadJapanese = false;
        ForceRenew = false;
        TestGEtFileWithProgress();
    }

    public void LoadEnglish4()
    {
        bundleName = "lvl4";
        LoadBigData = false;
        nowLoadJapanese = false;
        ForceRenew = false;
        TestGEtFileWithProgress();
    }
    public void ReLoadEnglish()
    {
        bundleName = "lvl3";
        LoadBigData = false;
        nowLoadJapanese = false;
        ForceRenew = true;
        TestGEtFileWithProgress();
    }

    public void LoadJapanes()
    {
        bundleName = "lvl4";
        LoadBigData = false;
        nowLoadJapanese = true;
        ForceRenew = false;
        TestGEtFileWithProgress();
    }
    public void ReLoadJapanes()
    {
        bundleName = "lvl4";
        LoadBigData = false;
        nowLoadJapanese = true;
        ForceRenew = true;
        TestGEtFileWithProgress();
    }
    public void LLoadBigData()
    {
        bundleName = "bigdataobject";
        LoadBigData = true;
        nowLoadJapanese = false;
        ForceRenew = false;
        TestGEtFileWithProgress();
    }

    public void ClearAllCache()
    {

        instanceOfBundles.UnloadAllMemorysAndValidate();

        // UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();


        if (Caching.ClearCache())
        {
            Debug.Log(" Cleared Cache! ");
        }
        else
        {
            Debug.Log(" Cache is not cleared!");
        }
    }

    public void ReLoadBigData()
    {
        bundleName = "bigdataobject";
        LoadBigData = true;
        nowLoadJapanese = false;
        ForceRenew = true;
        TestGEtFileWithProgress();
    }

#if UNITY_EDITOR
    // [MenuItem("Test/testIfBundleAndcaching")]
    // public static void TestBundleCaching()
    // {
    //     //
    //     var instance = FindObjectOfType<test>();
    //     instance.instanceOfBundles.IsBundleCached("lvl4");
    // }
    // [MenuItem("Test/testClearbundleFromStreamingAndcaching")]
    public static void TestBundleCachingClear()
    {
        bool cleared = Caching.ClearAllCachedVersions("lvl4".ToLower());
        Debug.Log(" Was it cleared? : " + cleared);

    }


    // public static void TestChangeCaching()
    // {

    //     string nameOfCachedBudle = "lvl4";
    //     var listOfPathss = new List<string>();
    //     Caching.GetAllCachePaths(listOfPathss);

    //     string pathToBundle = listOfPathss[0] + "/" + nameOfCachedBudle;
    //     if (!System.IO.Directory.Exists(pathToBundle))
    //     {
    //         Debug.Log(" Folder does not exists");
    //         var infoAboutDirectory = System.IO.Directory.CreateDirectory(pathToBundle);
    //         Debug.Log(" created into : " + infoAboutDirectory.FullName);
    //         while (!System.IO.Directory.Exists(pathToBundle))
    //         {
    //             // Wait
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log(" Folder exists!");
    //     }
    //         var someNewCache = Caching.AddCache(nameOfCachedBudle);
    //         // Move cache to the start of the queue
    //         Caching.MoveCacheBefore(someNewCache, Caching.GetCacheAt(0));
    //         Caching.currentCacheForWriting = someNewCache;
    // }
#endif
    //     // [MenuItem("Test/testGetMetadata")]
    public static void TestGEtUrl()
    {
        var instanceMono = FindObjectOfType<TestBundleService>();
        Instance = instanceMono;
        bytsFull = 0;
        Instance.StartCoroutine(Instance.justGetTheMetadata());

        // instanceMono.instanceOfBundles.Initialize(instanceMono);
        // instanceMono.StartCoroutine(instanceMono.instanceOfBundles.LoadbundleCoroutine("lvl3", instanceMono.OnComplete, instanceMono.OnError, instanceMono.OnLoadDep, instanceMono.OnLoadMain));
    }

    private IEnumerator justGetTheMetadata()
    {
        // firebase  alt=media& (load object), without - metadata
        int indexToFind = urlToloadFrom.IndexOf("alt=media&");
        string substringFirst;
        if (indexToFind > 0)
        {
            substringFirst = urlToloadFrom.Substring(0, indexToFind);
            substringFirst += urlToloadFrom.Substring(indexToFind + "alt=media&".Length);
            urlToloadFrom = substringFirst;

        }
        else
        {
            Debug.Log(" URL is wrong!");
            yield break;
        }
        Debug.Log("URL :" + urlToloadFrom); // remember to trim!!!!!!  alt=media
        using (var webRequest = new UnityWebRequest(urlToloadFrom))
        {
            webRequest.SendWebRequest();
            yield return webRequest;

            if (string.IsNullOrEmpty(webRequest.error))
            {

                // var allresponse = webRequest.responseHeaders;
                // if (allresponse != null)
                // {
                //     Debug.Log(" -------------> Response headers:");
                //     foreach (var item in allresponse.Keys)
                //     {
                //         Debug.Log(item + " : " + allresponse[item]);
                //     }
                // }
                // else
                // {
                //     Debug.Log("-----------> No  Headers were found!");
                // }
                Debug.Log(" -----------> Bytes downloaded Code :" + webRequest.downloadedBytes);

                var allObjects = webRequest.downloadHandler.text;
                var charArr = allObjects.ToCharArray();
                int indexFound = allObjects.IndexOf("\"size\"");

                string sizeString = "";
                if (indexFound > 0)
                {
                    int i = indexFound;
                    while (!char.IsDigit(charArr[i]))
                    {
                        i++;
                    }
                    while (char.IsDigit(charArr[i]))
                    {
                        sizeString += charArr[i];
                        i++;
                    }

                }
                Debug.Log(" -----------> Bytes :" + sizeString);
                ulong newSize = 0;
                // bytsFull
                ulong.TryParse(sizeString, out newSize);
                bytsFull += newSize;
                // Debug.Log("------------_ From Json : " + item);
                // if (item.Contains("size"))
                // {
                //     ulong.TryParse(allObjects[item], out bytsFull); //(ulong)allObjects[item];
                // }

                // Debug.Log(" -------------------> " + bytsFull);

                Debug.Log(" ----------> " + ConvertKilobytesToKiloBytes(bytsFull));
            }
            else
            {
                Debug.Log(" Error while getting Head : " + webRequest.error);
            }
        }

    }

    static double ConvertBytesToMegabytes(ulong bytes)
    {
        return (bytes / 1024f) / 1024f;
    }
    static double ConvertKilobytesToKiloBytes(ulong bytes)
    {
        return bytes / 1024f;
    }

    public void OnComplete(bool completed)
    {
        if (completed)
        {
            Debug.Log(" Completed!");
        }
        else
        {

            Debug.Log(" Not Completed");
        }
    }
    public void OnError(string completed)
    {

        Debug.Log("Error :" + completed);

    }

    public void OnLoadDep(float progress)
    {
        // Debug.Log(" Dependency : " + progress);
    }
    public void OnLoadMain(float progress)
    {
        // Debug.Log(" Main : " + progress);
    }
    public float GetPercentageOfDataCustom(ulong bytes)
    {
        if (bytsFull == 0)
        {
            return bytes;
        }
        return bytes / bytsFull;
    }

    [Header(" Force renew ")]
    public bool ForceRenew = false;

    public bool nowLoadJapanese = false;
    public bool LoadBigData = false;
    public string bundleName = "lvl3";

#if UNITY_EDITOR
    [MenuItem("Test/testGetFileData")]
#endif
    public static async void TestGEtFileWithProgress()
    {
        var instanceMono = FindObjectOfType<TestBundleService>();
        Instance = instanceMono;

        instanceMono.instanceOfBundles.Init();
        string prefix = null;
        BundleOptions myOptions = new BundleOptions();
        string bundleName = instanceMono.bundleName;
        // limit on a single package is around 130 000 bytes

        if (instanceMono.nowLoadJapanese)
        {
            myOptions.LoadLanguage = "ja";
            prefix = "ja";
        }
        else
        {
            myOptions.LoadLanguage = "en";
            prefix = "en";
        }

        if (instanceMono.LoadBigData)
        {
            bundleName = "bigdataobject";
            prefix = null;

            myOptions.LoadBigdataObject = true;
        }


        Debug.Log(" Options Used :" + " bundleName " + bundleName + "prefix " + prefix + " , options " + myOptions.LoadLanguage + " BigData =" + myOptions.LoadBigdataObject);

        // If Strategy.NONE, used default from the Scriptable Object
        await instanceMono.instanceOfBundles.LoadAnybundleCoroutine(
             bundleName, instanceMono.ForceRenew, Strategy.NONE,
            instanceMono.OnComplete, instanceMono.OnError, instanceMono.OnLoadDep,
            instanceMono.OnLoadMain, Instance.GetPercentageOfDataCustom, prefix, myOptions);
    }


#if UNITY_EDITOR
    // ----------------------Testing Ground Firebase Database ---------------------------------------------------------------------------
    #region  Testing Ground Firebase Database
    [Serializable]
    public class DataClass
    {
        public string UserID;
        public int Points;
        public List<int> allNewLevels;
        public string LastTime;
    }

    public static void TestPushUrl()
    {
        var instanceMono = FindObjectOfType<TestBundleService>();
        instanceMono.instanceOfBundles.Init();
        DataClass myNewOne = new DataClass();
        myNewOne.UserID = "TESTID1";
        myNewOne.Points = 100;
        myNewOne.allNewLevels = new List<int>();
        myNewOne.allNewLevels.Add(0);
        myNewOne.allNewLevels.Add(1);
        myNewOne.allNewLevels.Add(20);

        myNewOne.LastTime = DateTime.UtcNow.ToString("s");


        instanceMono.StartCoroutine(instanceMono.PushNewDataViaUrl(myNewOne, myNewOne.UserID));
    }
    /// <summary>
    /// Functionality for Removing Fields from Json string
    /// </summary>
    public static void TestUpdateUrl()
    {
        var instanceMono = FindObjectOfType<TestBundleService>();
        instanceMono.instanceOfBundles.Init();
        DataClass myNewOne = new DataClass();
        myNewOne.UserID = "TESTID1";
        myNewOne.Points = 20;
        myNewOne.allNewLevels = new List<int>();
        myNewOne.allNewLevels.Add(0);
        myNewOne.allNewLevels.Add(1);
        myNewOne.allNewLevels.Add(20);

        myNewOne.LastTime = DateTime.UtcNow.ToString("s");

        string wholeObject = JsonUtility.ToJson(myNewOne);
        Debug.Log(wholeObject);

        // string toDelete = null;
        for (int i = 0; i < fieldForData.Length; i++)
        {
            // Old Data == newData - remove from string
            // toDelete = fieldForData[i];
            switch (i)
            {
                case 0:
                    wholeObject = RemovedField(wholeObject, i);
                    Debug.Log(wholeObject);
                    break;
                case 2:
                    wholeObject = RemovedField(wholeObject, i);
                    Debug.Log(wholeObject);

                    break;
            }
        }

        instanceMono.StartCoroutine(instanceMono.UpdateDataViaUrl(wholeObject, myNewOne.UserID));
    }


    /// <summary>
    /// Functionality for Removing Fields from Json string
    /// </summary>
    private static string RemovedField(string originalJson, int fieldToRemove)
    {
        string newS = "";
        string fieldToRemoveString = fieldForData[fieldToRemove];
        int findStartOfField = originalJson.IndexOf("\"" + fieldToRemoveString + "\"");
        if (findStartOfField < 0)
        {
            return originalJson;
        }

        // found field to remove

        // case : Remove Field 0
        newS = originalJson.Substring(0, findStartOfField);
        if (findStartOfField == fieldForData.Length - 1)
        {
            // need to delete the ,
            newS = newS.Substring(0, newS.Length - 1);
            newS += "}";
            return newS;
        }
        // find next real property
        for (int i = fieldToRemove + 1; i < fieldForData.Length; i++)
        {
            findStartOfField = originalJson.IndexOf("\"" + fieldForData[i] + "\"");
            if (findStartOfField >= 0)
            {
                newS += originalJson.Substring(findStartOfField, originalJson.Length - findStartOfField);
                break;
            }
        }


        return newS;
    }


    /// <summary>
    /// Functionality for Removing Fields from Json string
    /// </summary>
    private static string[] fieldForData = new string[] { "UserID", "Points", "allNewLevels", "LastTime" };

    // ---------------------------------------------- Firebase 
    /// <summary>
    /// Works without any of the Firebase plugins (Pure Web requests) 
    /// </summary>
    public IEnumerator PushNewDataViaUrl(TestBundleService.DataClass dataToPush, string UserId)
    {
        // For Firebase
        // append .json at the end for Rest APi Firebase (Get request,only read,
        // Put request (Post will generate intermidiate key))
        string key = UserId;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield break;
        }
        string toString = JsonUtility.ToJson(dataToPush);

        using (var www = UnityWebRequest.Put("baseUrl" + key + ".json", toString)) // (baseUrl + keyNew + ".json")
        {
            // www.SetRequestHeader("Content-Type", "application/json"); // required if used Post

            yield return www.SendWebRequest();
            bool Errors = www.isNetworkError || www.isHttpError;

            if (!Errors)
            {

                Debug.Log("Pushed : " + (string)www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error in Downloading");
                Debug.Log(www.error);
                Debug.Log(www.responseCode);
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Update Database at location! Works without any of the Firebase plugins (Pure Web requests) 
    /// </summary>
    public IEnumerator UpdateDataViaUrl(string dataToPush, string UserId)
    {
        // For Firebase
        // append .json at the end for Rest APi Firebase (Get request,only read,
        // Put request (Post will generate intermidiate key))
        string key = UserId;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield break;
        }

        using (var www = new UnityWebRequest("baseUrl" + key + ".json", "PATCH")) // (baseUrl + keyNew + ".json")
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(dataToPush);
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            bool Errors = www.isNetworkError || www.isHttpError;

            if (!Errors)
            {

                Debug.Log("Pushed : " + (string)www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Error in Downloading");
                Debug.Log(www.error);
                Debug.Log(www.responseCode);
                Debug.Log(www.downloadHandler.text);
            }

        }

    }
    #endregion // Testing Ground Firebase Database
#endif
}


