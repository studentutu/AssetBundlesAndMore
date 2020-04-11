using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KAUGamesLviv.Services.Bundles
{
    [Serializable]
    public class UrlAsStringReference
    {
        public string url;

        public override string ToString()
        {
            return url;
        }
    }
    public class BundleScriptable<T> : BehaviourScriptableObject where T : BehaviourScriptableObject
    {
        #region  Hidden Data
        [SerializeField] protected BundleDependencies currentDependencies;
        protected Dictionary<string, AssetBundle> allLoadedToMemBundles = new Dictionary<string, AssetBundle>();
        protected Dictionary<string, List<string>> dependencyDictionary = new Dictionary<string, List<string>>();
        protected HashSet<string> noPrefixDeps = new HashSet<string>();
        [SerializeField] protected string DatabaseURL = ""; // Set your values for GetUrl method (you should implement it) !
        [SerializeField] protected List<StreamingAssetsItems> InStreamingAssets = new List<StreamingAssetsItems>();

        #endregion // Hidden Data


        #region Constructor_fields Dependencies
        protected string platformToUse;
        [SerializeField] public Strategy strategyToUse = Strategy.ALWAYS_UPDATE;

        [Tooltip("Enable this option if you want to use your Custom Logic for Mapping Bundle Names. e.g. using the same bundle names are not supported when set to false")]
        [SerializeField] private bool UseOwnMapingOfNames = false;

        #endregion

        #region Init Dependencies

        private void InitDependencies(BundleDependencies isItNew = null)
        {
            if (isItNew != null)
            {
                currentDependencies = isItNew;
            }
            dependencyDictionary = currentDependencies.ToDictionary();
            if (UseOwnMapingOfNames)
            {
                noPrefixDeps = currentDependencies.ToHelpHashSetForOwnMappingDeps();
            }
        }
        #endregion

        #region  Public Utility methods

        /// <summary> Always initialize! </summary>
        public void Constructed()
        {
#if UNITY_ANDROID
            platformToUse = "Android";
#elif UNITY_IOS
            platformToUse = "IOS";
#elif UNITY_EDITOR
            platformToUse = "Android";
#endif      
            if (!DatabaseURL.EndsWith("/"))
            {
                DatabaseURL += "/";
            }
            InitDependencies();
        }

        /// <summary> 
        /// Check if the Bundle already loaded into memory
        /// <para>If needed custom Mapping - enable UseOwnMapingOfNames and provide Prefix when loading the bundle</para>
        /// </summary>
        public bool IsBundleLoadedToMemory(string bundleName, string prefix = null)
        {
            if (UseOwnMapingOfNames && !string.IsNullOrEmpty(prefix))
            {
                // Provide prefix to bundlename when adding it to Dictionary!
                return allLoadedToMemBundles.ContainsKey(bundleName.ToLower() + prefix);

            }
            return allLoadedToMemBundles.ContainsKey(bundleName.ToLower());
        }


        /// <summary>
        /// Returns the requested object from the bundle 
        /// if not successfull, will return default value
        /// <para> If needed custom Mapping - enable UseOwnMapingOfNames and provide Prefix when loading the bundle</para>
        /// </summary>
        /// <param name="nameOfRequestedObject"> Name of requested Object (do not include .asset) </param>
        public object GetTheAssetFromBundle(
            string nameOfRequestedObject,
            string bundleName,
            string BundlePrefix = null)
        {
            object something = null;
            nameOfRequestedObject = nameOfRequestedObject.ToLower();
            if (IsBundleLoadedToMemory(bundleName, BundlePrefix))
            {
                string BundleInMem;
                // may be returned from the Bundle
                if (UseOwnMapingOfNames && !string.IsNullOrEmpty(BundlePrefix))
                {
                    // Provide prefix to bundlename when adding it to Dictionary!
                    BundleInMem = bundleName.ToLower() + BundlePrefix;
                }
                else
                {
                    BundleInMem = bundleName.ToLower();
                }

                var temporal = allLoadedToMemBundles[BundleInMem];
                string pathToAsset = "";

                // all bundles names has only lowercase Characters!
                pathToAsset += string.Format("{0}", nameOfRequestedObject);
                // Debug.Log(" Path to asset : " + pathToAsset);
                something = temporal.LoadAsset(pathToAsset);
            }

            return something;
        }


        /// <summary>
        /// Returns the requested sceneName from the bundle (You can load scene with SceneManager.LoadScene(name) ) 
        /// if not successfull, will return null 
        /// <para> If needed custom Mapping - enable UseOwnMapingOfNames and provide Prefix when loading the bundle</para>
        /// </summary>
        /// <param name="nameOfRequestedBundle"> Name of requested Bundle (do not include any extensions) </param>
        /// <returns> Return SceneName ready to load, check if it null </returns>
        public string GetTheSceneFromBundle(
            string nameOfRequestedBundle,
            string BundlePrefix = null)
        {
            string sceneLoadedFromBundle = null;
            string BundleInMem = nameOfRequestedBundle.ToLower();
            if (IsBundleLoadedToMemory(BundleInMem, BundlePrefix))
            {
                // may be returned from the Bundle
                if (UseOwnMapingOfNames && !string.IsNullOrEmpty(BundlePrefix))
                {
                    // Provide prefix to bundlename when adding it to Dictionary!
                    BundleInMem = nameOfRequestedBundle.ToLower() + BundlePrefix;
                }
                else
                {
                    BundleInMem = nameOfRequestedBundle.ToLower();
                }

                var temporal = allLoadedToMemBundles[BundleInMem];
                if (temporal.isStreamedSceneAssetBundle)
                {
                    string[] scenePaths = temporal.GetAllScenePaths();
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePaths[0]);
                    sceneLoadedFromBundle = sceneName;
                }
            }
            return sceneLoadedFromBundle;
        }

        public void RemoveFromMemory(string BundleName)
        {
            string BundleInMem = BundleName.ToLower();

            // Delete from Memory  in Any Case to prevent Unity Same Memory Error
            if (UseOwnMapingOfNames)
            {
                // Search others!!
                var ListOfKeys = new List<string>();
                foreach (var item in allLoadedToMemBundles.Keys)
                {
                    ListOfKeys.Add(item);
                }
                foreach (var item in ListOfKeys)
                {
                    if (item.StartsWith(BundleInMem))
                    {
                        allLoadedToMemBundles[item].Unload(true);
                        allLoadedToMemBundles.Remove(item);
                    }
                }
            }
            else
            {
                if (IsBundleLoadedToMemory(BundleInMem))
                {
                    allLoadedToMemBundles[BundleInMem.ToLower()].Unload(true);
                    allLoadedToMemBundles.Remove(BundleInMem.ToLower());
                }
            }
        }

        public void ValidateMemoryBundles()
        {
            var ListOfKeys = new List<string>();
            foreach (var item in allLoadedToMemBundles.Keys)
            {
                ListOfKeys.Add(item);
            }
            foreach (var item in ListOfKeys)
            {
                if (allLoadedToMemBundles[item] == null)
                {
                    allLoadedToMemBundles.Remove(item);
                }
            }

            string LoadedBundles = "Bundles Now In Memory: ";
            ListOfKeys.Clear();
            foreach (var item in allLoadedToMemBundles.Keys)
            {
                LoadedBundles += item + " ";
            }
            Log(LoadedBundles);
        }

        public void UnloadAllMemorysAndValidate()
        {
            var ListOfKeys = new List<string>();
            foreach (var item in allLoadedToMemBundles.Keys)
            {
                ListOfKeys.Add(item);
            }
            foreach (var item in ListOfKeys)
            {
                if (allLoadedToMemBundles[item] != null)
                {
                    allLoadedToMemBundles[item].Unload(true);
                    allLoadedToMemBundles.Remove(item);
                }
            }
            AssetBundle.UnloadAllAssetBundles(true);
            ValidateMemoryBundles();
        }

        public bool CheckIfBundleIsCached(string bundleName, string prefix, System.Object options)
        {
            return IsBundleCached(bundleName, prefix, options);
        }

        public string IsInStreamingAssets(string bundleName, string Prefix)
        {
            string strmAssetspath = null;
            string usePrefix = noPrefixDeps.Contains(bundleName) ? null : Prefix;

            if (UseOwnMapingOfNames)
            {
                if (string.IsNullOrEmpty(usePrefix))
                {
                    foreach (var item in InStreamingAssets)
                    {
                        if (item.bundleName.Equals(bundleName))
                        {
#if UNITY_ANDROID
                            strmAssetspath = item.bundlePathAndroid;
#else
                            strmAssetspath = item.bundlePathIOS;
#endif
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var item in InStreamingAssets)
                    {
                        if (item.bundleName.Equals(bundleName + usePrefix))
                        {
#if UNITY_ANDROID
                            strmAssetspath = item.bundlePathAndroid;
#else
                            strmAssetspath = item.bundlePathIOS;
#endif
                            break;
                        }
                    }

                }
            }
            else
            {
                // - Limitations:
                //               - works only if the BundleName is Unique 
                foreach (var item in InStreamingAssets)
                {
                    if (item.bundleName.Equals(bundleName))
                    {
#if UNITY_ANDROID
                        strmAssetspath = item.bundlePathAndroid;
#else
                        strmAssetspath = item.bundlePathIOS;
#endif
                        break;
                    }
                }
            }
            return strmAssetspath;
        }
        #endregion //  Public methods


        #region Hidden Methods

        ///<summary> Log Entry. Will not do anything if not in Editor </summary>
        protected static void Log(object message)
        {
            // #if UNITY_EDITOR
            Debug.Log(string.Format("<Color=Blue> {0} </Color>", message));
            // #endif
        }

        ///<summary> Log Entry. Will not do anything if not in Editor </summary>
        protected static void LogWarning(object message)
        {
            // #if UNITY_EDITOR
            Debug.LogWarning(string.Format("<Color=Yellow> {0} </Color>", message));
            // #endif
        }


        private bool IsDependencyContainsGivenBundle(string bundleMain, string prefix = null)
        {
            if (UseOwnMapingOfNames && !string.IsNullOrEmpty(prefix))
            {
                return dependencyDictionary.ContainsKey(bundleMain + prefix);
            }
            return dependencyDictionary.ContainsKey(bundleMain);
        }


        private List<string> ReceiveDependencies(string bundleMain, string prefix = null)
        {
            if (UseOwnMapingOfNames && !string.IsNullOrEmpty(prefix))
            {
                return dependencyDictionary[bundleMain + prefix];
            }
            return dependencyDictionary[bundleMain];
        }


        /// <summary>
        /// Returns true only if the Bundle is found in Cache
        /// <para>If needed custom Mapping - enable UseOwnMapingOfNames and override BundleNameToCacheName(bundleName)</para>
        /// </summary>
        /// <param name="nameOfRequestedBundle"> Name of requested Bundle (do not include any extensions) </param>
        private bool IsBundleCached(string bundleName, string prefix, object options)
        {
            bool cachedFromStreamingAssets = false;
            if (!string.IsNullOrEmpty(IsInStreamingAssets(bundleName.ToLower(), prefix)))
            {
                // Also check the simpler name of bundle when Loaded From Streaming Assets
                // Check exactly the name!
                cachedFromStreamingAssets = CheckBundleNameExactInCache(bundleName);
            }

            if (cachedFromStreamingAssets)
                return true;


            string cachedNameFromCustoms = bundleName.ToLower();
            if (UseOwnMapingOfNames)
            {
                cachedNameFromCustoms = BundleNameToCacheName(bundleName.ToLower(), options);
            }

            // This part will search the whole Default Cache Directory for a folder that contains bundleName
            // - Limitations with Non Custom name Mapping :
            //               - works only if the BundleName is Unique
            //               - check is not exact!
            List<string> allCachedPath = new List<string>();
            Caching.GetAllCachePaths(allCachedPath);
#if UNITY_EDITOR
            foreach (var item in allCachedPath)
            {
                Log(" ----------> Cache at : " + item);
            }
#endif
            if (allCachedPath.Count == 0)
            {
                return false;
            }

            var path = allCachedPath[0];
            if (!Directory.Exists(path))
            {
                return false;
            }
            DirectoryInfo someInfo = new DirectoryInfo(path);
            var allDirectoriesInside = someInfo.GetDirectories();
            bool exists = false;
            int numberOfDirectories = 0;

            foreach (var item in allDirectoriesInside)
            {
                numberOfDirectories = 0;
                if (item.FullName.Contains(cachedNameFromCustoms))
                {
                    numberOfDirectories = item.GetDirectories().Length;
                    // Number of directories are all of the versions of the bundle
                    if (numberOfDirectories > 0)
                    {
                        exists = true;
#if UNITY_EDITOR
                        Log(" --------> Found from Directory ");
#endif
                    }
                }
            }

            return exists;
        }

        private bool CheckBundleNameExactInCache(string bundleName)
        {
            string cachedNameFromCustoms = bundleName.ToLower();
            // This part will search the whole Default Cache Directory for a folder that contains bundleName
            // - Limitations with Non Custom name Mapping :
            //               - works only if the BundleName is Unique
            List<string> allCachedPath = new List<string>();
            Caching.GetAllCachePaths(allCachedPath);

            if (allCachedPath.Count == 0)
            {
                return false;
            }

            var path = allCachedPath[0];
            if (!Directory.Exists(path))
            {
                return false;
            }
            DirectoryInfo someInfo = new DirectoryInfo(path);

            var allDirectoriesInside = someInfo.GetDirectories();
            bool exists = false;
            int numberOfDirectories = 0;

            // Beware of slashes!
            foreach (var item in allDirectoriesInside)
            {
                numberOfDirectories = 0;

                if (item.FullName.EndsWith("/" + cachedNameFromCustoms))
                {
                    numberOfDirectories = item.GetDirectories().Length;
                    // Number of directories are all of the versions of the bundle
                    if (numberOfDirectories > 0)
                    {
                        exists = true;
#if UNITY_EDITOR
                        Log(" --------> Exact Name is Streaming Cache ");
#endif
                    }
                }

                if (item.FullName.EndsWith("\\" + cachedNameFromCustoms))
                {
                    numberOfDirectories = item.GetDirectories().Length;
                    // Number of directories are all of the versions of the bundle
                    if (numberOfDirectories > 0)
                    {
                        exists = true;
#if UNITY_EDITOR
                        Log(" --------> Exact Name is Streaming Cache ");
#endif
                    }
                }
            }

            return exists;
        }


        /// <summary>
        /// Returns string with cached name of the bundle, check if it is found previously.
        /// Sometimes Unity will add part of the URL as cache name like slashes, ampersands etc.
        /// <para>If needed custom Mapping - enable UseOwnMapingOfNames and override BundleNameToCacheName(bundleName)</para>
        /// </summary>
        /// <param name="bundleName"> Name of requested Bundle (do not include any extensions) </param>
        protected string GetCachedNameOfBundle(string bundleName, object options)
        {
            string result = null;
            if (UseOwnMapingOfNames)
            {
                result = BundleNameToCacheName(bundleName.ToLower(), options);
            }
            else
            {
                // This part will search the whole Default Cache Directory for a folder that contains bundleName
                // - Limitations:
                //               - check before if the folder even exists!
                //               - works only if the BundleName is Unique
                List<string> allCachedPath = new List<string>();
                Caching.GetAllCachePaths(allCachedPath);
#if UNITY_EDITOR
                foreach (var item in allCachedPath)
                {
                    Log(" ----------> You can find Cache at : " + item);
                }
#endif
                var path = allCachedPath[0];
                DirectoryInfo someInfo = new DirectoryInfo(path);
                var allDirectoriesInside = someInfo.GetDirectories();
                int numberOfDirectories = 0;

                foreach (var item in allDirectoriesInside)
                {
                    numberOfDirectories = 0;
                    // Works for Editor
                    if (item.FullName.Contains(bundleName.ToLower()))
                    {
                        numberOfDirectories = item.GetDirectories().Length;
                        // Check if it is truly cached - when cached, it will have a directory!
                        if (numberOfDirectories > 0)
                        {
                            int indexOfLast = item.FullName.LastIndexOf("/");
                            if (indexOfLast < 0)
                            {
                                indexOfLast = item.FullName.LastIndexOf("\\");
                            }
                            if (indexOfLast < 0)
                            {
                                Log(" Have not found any slashes!");
                                result = item.FullName;
                            }
                            else
                            {
                                result = item.FullName.Substring(indexOfLast);
                                if (result.StartsWith("/") || result.StartsWith("\\"))
                                {
                                    result = result.Substring(1);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            Log(" --------> Cached name is : " + result);
            return result;
        }

        /// <summary>
        /// Checks all of the cache for a bundle from a given url
        /// </summary>
        protected bool CheckForCacheBundleFromUrl(
            string bundleToSearch,
            string urlToSearch,
            object options,
            string prefix)
        {
            bool found = false;
            var allHashes = new List<Hash128>();
            Log("----------> Searching in Cache via URL!");
            bool cachedFromStreamingAssets = false;
            if (!string.IsNullOrEmpty(IsInStreamingAssets(bundleToSearch.ToLower(), prefix)))
            {
                // Also check the simpler name of bundle when Loaded From Streaming Assets
                // Check exactly the name!
                cachedFromStreamingAssets = CheckBundleNameExactInCache(bundleToSearch);
            }
            if (cachedFromStreamingAssets)
            {
                Caching.GetCachedVersions(bundleToSearch.ToLower(), allHashes);
            }
            else
            {
                string checkIfNull = GetCachedNameOfBundle(bundleToSearch, options);
                if (!string.IsNullOrEmpty(checkIfNull))
                    Caching.GetCachedVersions(GetCachedNameOfBundle(bundleToSearch, options), allHashes);
            }

            foreach (var item in allHashes)
            {
                if (!Caching.IsVersionCached(urlToSearch, item)) continue;
                Log(" Found in cache! : " + bundleToSearch);
                found = true;
            }

            return found;
        }

        // It will load streaming asset bundle into cache folder!
        // bundlename should be in lowercase!
        private async Task LoadBundlefromStreamingAsset(string pathFromStreamingAssets, string bundleName)
        {
            string pathToBundle = "file:///" + Application.streamingAssetsPath + "/" + pathFromStreamingAssets;
#if UNITY_ANDROID
            // pathToBundle = Application.streamingAssetsPath + "/" + pathFromStreamingAssets;
            pathToBundle = System.IO.Path.Combine(Application.streamingAssetsPath, pathFromStreamingAssets);
#endif
            Log(" Loading From Streaming Assets into cache: " + pathToBundle);

            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(pathToBundle, 0, 0))
            {
                await request.SendWebRequest();

                while (!request.isDone && !request.isNetworkError && !request.isHttpError)
                {
                    await Task.Yield();
                }

                //get_assetBundle can only be called from the main thread.                
                AssetBundle bundle = ((UnityEngine.Networking.DownloadHandlerAssetBundle)request.downloadHandler).assetBundle;
                // UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);

                Log(" Loading From Streaming Assets into cache ? : " + ((bundle == null) ? "Failure" : "Succes"));
                // Remove from memory to allow Caching logic do it's job
                if (bundle != null)
                {
                    bundle.Unload(true);
                }
            }
        }
        #endregion // Hidden Methods


        #region  Public Load Bundle method  (Should not be changed!)
        ///<summary> Entry to Load Bundle. Limitation - each bundle should have different name if used with non-Custom implementation! </summary>
        public async Task LoadAnybundleCoroutine(
          string bundleName,
          bool loadAnew = false,
          Strategy strategyUsed = Strategy.NONE,
          Action<bool> onComplete = null,
          Action<string> onError = null,
          Action<float> onLoadingDependencies = null,
          Action<float> onLoadingMain = null,
          Func<ulong, float> customProgressFunction = null,
          string Prefix = null,
          object options = null)
        {

            if (strategyUsed == Strategy.NONE)
            {
                strategyUsed = strategyToUse;
            }
            var currentURL = new UrlAsStringReference();
            bool online = true;

            // If loadAnew == false, will check if it is already loaded, if so, terminate
            if (!loadAnew)
            {
                // If needed custom Mapping :
                //                           - enable UseOwnMapingOfNames 
                //                           - override BundleNameToCacheName(bundleName)
                //                           - provide Prefix string
                if (IsBundleLoadedToMemory(bundleName, Prefix))
                {
                    if (onComplete != null)
                    {
                        onComplete.Invoke(true);
                    }
                    return;
                }

            }

            // Delete from Memory  in Any Case to prevent Unity  Same Memory Error
            if (UseOwnMapingOfNames)
            {
                // Search others!!
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(Prefix))
                {
                    if (!noPrefixDeps.Contains(bundleName))
                        LogWarning(" Prefix is null or Empty when using custom Mapping!");
                }
#endif
                var ListOfKeys = new List<string>();
                foreach (var item in allLoadedToMemBundles.Keys)
                {
                    ListOfKeys.Add(item);
                }
                foreach (var item in ListOfKeys)
                {
                    if (item.StartsWith(bundleName))
                    {
                        allLoadedToMemBundles[item].Unload(true);
                        allLoadedToMemBundles.Remove(item);
                    }
                }
            }
            else
            {
                if (IsBundleLoadedToMemory(bundleName))
                {
                    allLoadedToMemBundles[bundleName.ToLower()].Unload(true);
                    allLoadedToMemBundles.Remove(bundleName.ToLower());
                }

            }

            Resources.UnloadUnusedAssets(); // do you need it ???? It will clear the resources, but  will result in a Frame Hiccup

            while (!Caching.ready)
            {
                await Task.Yield();
            }

            bool isItCached = IsBundleCached(bundleName, Prefix, options);

            if (UseOwnMapingOfNames)
            {
                // Two identical bundle names with different content!
                // Set Cached Bundle Name into PlayerPrefs and Check if value == Prefix!
                if (PlayerPrefs.HasKey(GetCachedNameOfBundle(bundleName, options)))
                {
                    string usePrefix = noPrefixDeps.Contains(bundleName) ? null : Prefix;
                    if (usePrefix != null && PlayerPrefs.GetString(GetCachedNameOfBundle(bundleName, options)) != Prefix)
                    {
                        Log(" Check with Collision on the same name with Player Prefs");
                        isItCached = false;
                        if (CheckBundleNameExactInCache(GetCachedNameOfBundle(bundleName, options)))
                        {
                            isItCached = true;
                        }
                    }
                }
            }

            // If not cached, try to load from Streaming Assets
            if (!isItCached)
            {
                string StreamingAssetPath = IsInStreamingAssets(bundleName.ToLower(), Prefix);
                if (StreamingAssetPath != null)
                {
                    // Will load asset bundle into memory and into caching!
                    await LoadBundlefromStreamingAsset(StreamingAssetPath, bundleName);
                    isItCached = IsBundleCached(bundleName, Prefix, options);
                }
            }

            Log(bundleName + " is cached ? " + isItCached);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                online = false;
            }
            if (!online && !isItCached)
            {
                if (onComplete != null)
                {
                    onComplete.Invoke(false);
                }
                if (onError != null)
                {
                    LogWarning("Bundle was not loaded and we have no Connection!");
                    onError.Invoke("Bundle was not loaded and we have no Connection!");
                }
                return;
            }

            // Strategy to use
            if (isItCached && online && strategyUsed == Strategy.ALWAYS_UPDATE || isItCached && online && loadAnew)
            {
                // Get currentURL 
                Log(" -------------> Overwrite bundle with new Version (or the same) !");
                await GetUrlCustom(bundleName, currentURL, options);

                if (string.IsNullOrEmpty(currentURL.url))
                {
                    // load from cache if failed
                    LogWarning(" URL is wrong, will Load from Cache instead");
                    await LoadingMainBundlewithDependsFromCache(bundleName,
                                                                onComplete, onError,
                                                                onLoadingDependencies,
                                                                onLoadingMain,
                                                                customProgressFunction,
                                                                Prefix, options);

                    return;
                }

                // Delete from Cache, because of the same Bundle Naming even in Normal Mode!
                bool cleared = Caching.ClearAllCachedVersions(
                    GetCachedNameOfBundle(bundleName, options));
                if (!cleared)
                {
                    if (onComplete != null)
                    {
                        onComplete.Invoke(true);
                    }
                    if (onError != null)
                    {
                        LogWarning("Bundle is being used and cannot be Deleted from Cache!");
                        onError.Invoke("Bundle is being used and cannot be Deleted from Cache!");
                    }

                    return;
                }
                string StreamingAssetPath = IsInStreamingAssets(bundleName.ToLower(), Prefix);
                if (StreamingAssetPath != null)
                {
                    cleared = Caching.ClearAllCachedVersions(bundleName.ToLower());
                }

                // Start Loading:  Renew to the latest version
                await LoadingMainBundleWithDependsFromServer(online,
                                                            bundleName, currentURL, onComplete,
                                                            onError, onLoadingDependencies,
                                                            onLoadingMain, customProgressFunction,
                                                            Prefix, options);
                return;
            }


            if (isItCached)
            {
                await LoadingMainBundlewithDependsFromCache(bundleName,
                                                            onComplete, onError,
                                                            onLoadingDependencies,
                                                            onLoadingMain,
                                                            customProgressFunction,
                                                            Prefix, options);
            }
            else
            {
                // Just load new  Bundle (is not cached when in Normal Mode - each bundle has unique name)

                // For UseOwnMapingOfNames and as a precaution for normal Mode:  Delete from Cache, because of the same Naming!
                // Do the same prefixes
                string checkForNull = GetCachedNameOfBundle(bundleName, options);
                bool cleared = false;
                if (!string.IsNullOrEmpty(checkForNull))
                {

                    cleared = Caching.ClearAllCachedVersions(
                                GetCachedNameOfBundle(bundleName, options));
                    if (!cleared)
                    {
                        if (onComplete != null)
                        {
                            onComplete.Invoke(false);
                        }
                        if (onError != null)
                        {
                            LogWarning("Bundle is being used and cannot be Deleted from Cache!");
                            onError.Invoke("Bundle is being used and cannot be Deleted from Cache!");
                        }

                        return;
                    }

                }
                // Delete simpler name
                string StreamingAssetPath = IsInStreamingAssets(bundleName.ToLower(), Prefix);
                if (StreamingAssetPath != null)
                {
                    // when loaded from assetbundles, bundlename is always the same!
                    cleared = Caching.ClearAllCachedVersions(bundleName.ToLower());
                }

                await LoadingMainBundleWithDependsFromServer(online,
                                                            bundleName, currentURL, onComplete,
                                                            onError, onLoadingDependencies, onLoadingMain,
                                                            customProgressFunction, Prefix, options);
            }
        }

        #endregion // Public Load Bundle method  (Should not be changed!)

        #region Hidden Implementation (DO NOT TOUCH)
        /// <summary>
        /// Works without any of the Firebase plugins (Pure Web requests) 
        /// Behaviour for downloading Bundle from Cache
        /// </summary>
        private async Task LoadingMainBundlewithDependsFromCache(
            string bundleName,
            Action<bool> onComplete,
            Action<string> onError,
            Action<float> onLoadingDependencies,
            Action<float> onLoadingMain,
            Func<ulong, float> customProgressFunction,
            string Prefix,
            object options)
        {

            if (IsDependencyContainsGivenBundle(bundleName))
            {
                float depPercentage = 0;
                int allDepnumber = 0;
                List<string> allDependencies = ReceiveDependencies(bundleName);
                // List of strings of dependencies who should not have prefixes!
                int allDeps = allDependencies.Count;
                onLoadingDependencies.Invoke(depPercentage); // first invoke with null

                bool isItCached = false;
                foreach (var item in allDependencies)
                {
                    if (!IsBundleLoadedToMemory(item))
                    {
                        string usePrefix = noPrefixDeps.Contains(item) ? null : Prefix;
                        isItCached = IsBundleCached(bundleName, usePrefix, options);

                        if (UseOwnMapingOfNames)
                        {
                            // Two identical bundle names with different content!
                            // Set Cached Bundle Name into PlayerPrefs and Check if value == Prefix!
                            if (PlayerPrefs.HasKey(GetCachedNameOfBundle(bundleName, options)))
                            {
                                if (PlayerPrefs.GetString(GetCachedNameOfBundle(bundleName, options)) != Prefix)
                                {
                                    isItCached = false;
                                }
                            }
                        }

                        if (isItCached)
                        {
                            Log("dependency bundle :" + item + " is cached");
                            await LoadBundle(false, item, null, null, null, null, onError, null, usePrefix, null);
                        }
                        else
                        {
                            Log("dependency bundle :" + item + " needs to be loaded");
                            // Find the Url for a given Bundle Name
                            await LoadAnybundleCoroutine(item, false, Strategy.NONE, null, onError, null, null, null, usePrefix); // load anew, in this situation does nothing!
                                                                                                                                  // the main decision of downloading it all a new comes from Chosen Strategy (one for a whole process)!
                        }
                        if (IsBundleLoadedToMemory(item))
                        {
                            allDepnumber += 1;
                            depPercentage = allDepnumber / allDeps;
                            onLoadingDependencies.Invoke(depPercentage);
                        }
                    }
                    else
                    {
                        allDepnumber += 1;
                        depPercentage = allDepnumber / allDeps;
                        onLoadingDependencies.Invoke(depPercentage);
                    }
                }
            }
            // From Cache
            await LoadBundle(false, bundleName,
                            null, onComplete,
                            null, onLoadingMain,
                            onError, null,
                            Prefix, options);
        }

        /// <summary>
        /// Works without any of the Firebase plugins (Pure Web requests) 
        /// Behaviour for downloading Bundle from Server
        /// </summary>
        private async Task LoadingMainBundleWithDependsFromServer(
            bool online,
            string bundleName,
            UrlAsStringReference currentURL,
            Action<bool> onComplete,
            Action<string> onError,
            Action<float> onLoadingDependencies,
            Action<float> onLoadingMain,
            Func<ulong, float> customProgressFunction,
            string Prefix,
            object options)
        {
            Log("Game " + bundleName + " needs To be loaded");
            // Get CurrentUrl for bundle
            if (online)
            {
                await GetUrlCustom(bundleName, currentURL, options);
            }
            if (string.IsNullOrEmpty(currentURL.url))
            {
                if (onComplete != null)
                {
                    onComplete.Invoke(false);
                }
                if (onError != null)
                {
                    LogWarning(bundleName + " URL from server is not valid, Bundle is not cached");
                    onError.Invoke(" URL from server is not valid, Bundle is not cached");
                }
                return;
            }
            else
            {
                if (IsDependencyContainsGivenBundle(bundleName))
                {
                    float depPercentage = 0;
                    int allDepnumber = 0;
                    List<string> allDependencies = ReceiveDependencies(bundleName);
                    int allDeps = allDependencies.Count;
                    onLoadingDependencies.Invoke(depPercentage); // first invoke with null
                    bool isItCached = false;
                    foreach (var item in allDependencies)
                    {
                        if (!IsBundleLoadedToMemory(item))
                        {
                            string usePrefix = noPrefixDeps.Contains(item) ? null : Prefix;
                            isItCached = IsBundleCached(bundleName, usePrefix, options);

                            if (UseOwnMapingOfNames)
                            {
                                // Two identical bundle names with different content!
                                // Set Cached Bundle Name into PlayerPrefs and Check if value == Prefix!
                                if (PlayerPrefs.HasKey(GetCachedNameOfBundle(bundleName, options)))
                                {
                                    if (PlayerPrefs.GetString(
                                        GetCachedNameOfBundle(bundleName, options)) != Prefix)
                                    {
                                        isItCached = false;
                                    }
                                }
                            }

                            if (isItCached)
                            {
                                Log("dependency bundle :" + item + " is cached");
                                await LoadBundle(false, item,
                                                 null, null,
                                                 null, null,
                                                 onError, null,
                                                 usePrefix, null);
                            }
                            else
                            {
                                Log("dependency bundle :" + item + " needs to be loaded");
                                // Find the Url for a given Bundle Name
                                // load anew, in this situation does nothing!
                                await LoadAnybundleCoroutine(item, false,
                                                             Strategy.NONE,
                                                             null, onError,
                                                             null, null,
                                                             null, usePrefix);

                            }
                            if (IsBundleLoadedToMemory(item))
                            {
                                allDepnumber += 1;
                                depPercentage = allDepnumber / allDeps;
                                onLoadingDependencies.Invoke(depPercentage);
                            }
                        }
                        else
                        {
                            allDepnumber += 1;
                            depPercentage = allDepnumber / allDeps;
                            onLoadingDependencies.Invoke(depPercentage);
                        }
                    }
                }

                // start loading
                await LoadBundle(true, bundleName, currentURL, onComplete, null, onLoadingMain, onError, customProgressFunction, Prefix, options);
            }

        }


        /// <summary>
        /// Works without any of the Firebase plugins (Pure Web requests) 
        /// Offline safe (works offline with Cache)
        /// </summary>
        private async System.Threading.Tasks.Task LoadBundle(
            bool load,
            string BundleName,
            UrlAsStringReference currentURL,
            Action<bool> onComplete,
            Action onLoadingStart,
            Action<float> onLoading,
            Action<string> onError,
            Func<ulong, float> customprogress,
            string Prefix,
            System.Object options)
        {

            if (load)
            {
                Log("----------------> Start loading Data! " + currentURL.url);
                if (onLoadingStart != null)
                    onLoadingStart.Invoke();



                // This way it will always search cached values and if it fails to load from cache, will load from Server
                // Checks with Cache with verion 0, if it exists, will load from cache, if fails or not exists, will load from Server
                using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(currentURL.url, 0, 0))
                // using (var www = WWW.LoadFromCacheOrDownload(currentURL, 0)) // deprecated
                {

                    var theWebRequest = www.SendWebRequest();


                    while (www.downloadProgress < 1 && !(www.isNetworkError || www.isHttpError))
                    {
                        if (onLoading != null)
                            onLoading.Invoke(www.downloadProgress);

                        if (customprogress != null)
                        {
                            float values = customprogress(www.downloadedBytes);
                            Log(System.String.Format("Percentage: {0:P2}.", values));
                        }

                        await System.Threading.Tasks.Task.Delay(100);
                    }
                    if (onLoading != null)
                        onLoading.Invoke(www.downloadProgress);

                    await theWebRequest;

                    while (!www.isDone && !www.isNetworkError && !www.isHttpError)
                    {

                        await System.Threading.Tasks.Task.Delay(50);
                    }
                    // yield return new WaitForSeconds(0.25f); // For big and small data, decompression time
                    bool Errors = www.isNetworkError || www.isHttpError;
                    if (!Errors)
                    {
                        var loadedBundle = ((UnityEngine.Networking.DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
                        // UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(www);
                        if (UseOwnMapingOfNames)
                        {
                            // bundle names may be identical!
                            if (string.IsNullOrEmpty(Prefix))
                            {
                                allLoadedToMemBundles.Add(loadedBundle.name, loadedBundle);
                            }
                            else
                            {
                                allLoadedToMemBundles.Add(loadedBundle.name + Prefix, loadedBundle);
                            }
                            PlayerPrefs.SetString(GetCachedNameOfBundle(BundleName, options), Prefix);
                        }
                        else
                        {
                            // bundle names are unique!
                            allLoadedToMemBundles.Add(loadedBundle.name, loadedBundle);
                        }

                        Log("---> Bundle Registered!! : " + loadedBundle.name);


                        if (onComplete != null)
                            onComplete.Invoke(true);

                    }
                    else
                    {
                        Log("Error in Download : " + www.error);
                        if (onError != null)
                            onError.Invoke(www.error);

                        if (onComplete != null)
                            onComplete.Invoke(false);
                    }

                }
            }
            else
            {
                // will load from cache based on its cached name ( sometimes contains part of the link itself)
                CachedAssetBundle somethingToCheckWith = new CachedAssetBundle();
                somethingToCheckWith.name = GetCachedNameOfBundle(BundleName, options);

                // Either was loaded from URL or from Streaming Assets!
                bool cachedFromStreamingAssets = false;
                if (!string.IsNullOrEmpty(IsInStreamingAssets(BundleName.ToLower(), Prefix)))
                {
                    // Also check the simpler name of bundle when Loaded From Streaming Assets
                    cachedFromStreamingAssets = CheckBundleNameExactInCache(BundleName);
                }
                if (cachedFromStreamingAssets)
                {
                    somethingToCheckWith.name = BundleName.ToLower();
                }

                Log(" using Cached Link : " + somethingToCheckWith.name);
                using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle("", somethingToCheckWith, 0))
                {
                    await www.SendWebRequest();
                    // while (www.downloadProgress < 1 && string.IsNullOrEmpty( www.error))
                    // {
                    //     yield return null;
                    // }
                    // yield return www;

                    while (!www.isDone && !www.isNetworkError && !www.isHttpError)
                    {

                        await System.Threading.Tasks.Task.Delay(50);
                    }
                    // yield return new WaitForSeconds(0.25f); // needs a bit of time to process for downloadHandler
                    var loadedBundle = ((UnityEngine.Networking.DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
                    // UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(www);
                    bool NoErrors = loadedBundle != null && string.IsNullOrEmpty(www.error);
                    if (NoErrors)
                    {
                        if (UseOwnMapingOfNames)
                        {
                            // bundle names may be identical!
                            if (string.IsNullOrEmpty(Prefix))
                            {
                                allLoadedToMemBundles.Add(loadedBundle.name, loadedBundle);
                            }
                            else
                            {
                                allLoadedToMemBundles.Add(loadedBundle.name + Prefix, loadedBundle);
                            }
                        }
                        else
                        {
                            // bundle names are unique!
                            allLoadedToMemBundles.Add(loadedBundle.name, loadedBundle);
                        }

                        Log("---> Bundle Registered!! : " + loadedBundle.name);


                        if (onComplete != null)
                            onComplete.Invoke(true);

                    }
                    else
                    {
                        Log("Error in Downloading From Cache : " + www.error);
                        if (onError != null)
                            onError.Invoke(www.error);

                        if (onComplete != null)
                            onComplete.Invoke(false);
                    }

                }
            }
        }


        #endregion // Hidden Implementation (DO NOT TOUCH)


        #region Your Custom Implementation 
        /// <summary>
        /// Free to customize for every bundle name, DO NOT CALL base method!
        /// </summary>
        public virtual async System.Threading.Tasks.Task GetUrlCustom(string bundleName, UrlAsStringReference urlToSet, System.Object options)
        {
            // Example:  Standart Way
            Debug.Log("-----------------------------------> Standart Example. ");

            string language = "en";
            if (options != null)
            {
                language = ((BundleOptions)options).LoadLanguage;
            }
            string someUrl = null;
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

            using (var www = UnityEngine.Networking.UnityWebRequest.Get(DatabaseURL + keyNew)) // (baseUrl + keyNew)
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
                    Log("Error in Downloadin : " + www.error);
                    Log(www.responseCode);
                }

            }

        }


        /// <summary>
        /// DO NOT CALL base method!
        /// </summary>
        protected virtual string BundleNameToCacheName(string bundleName, System.Object options)
        {
            var prefix = "en";
            if (options != null)
            {
                prefix = ((BundleOptions)options).LoadLanguage;
            }
            // Example of implementing custom Mapping
            string pathInStorage = "Bundles%2F" + platformToUse + "%2F" + prefix + "%2F" + bundleName;

            return pathInStorage;
        }

        #endregion // Custom Implementation  if needed
    }

    #region  Serialization Utilities

    [System.Serializable]
    public class StreamingAssetsItems
    {
        public string bundleName;
        public string bundlePathAndroid;
        public string bundlePathIOS;
    }
    // Template class
    public class BehaviourScriptableObject : UnityEngine.ScriptableObject
    {

    }
    public enum Strategy
    {
        ALWAYS_UPDATE = 0,
        KEEP = 1,
        NONE = 2
    }

    #endregion
}