using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtilities : MonoBehaviour
{

    #region Tools_for_tests
    [MenuItem("Assets/ClearCachedFiles")]
    public static void ClearCache()
    {
        UnityEngine.AssetBundle.UnloadAllAssetBundles(true);

        if (UnityEngine.Caching.ClearCache())
        {
            UnityEngine.Debug.Log(" Cleared Cache! ");
        }
        else
        {
            UnityEngine.Debug.Log(" Cache is not cleared!");
        }
        UnityEngine.Resources.UnloadUnusedAssets();

    }
    [MenuItem("Assets/ClearPrefs")]
    public static void ClearPrefs()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        UnityEngine.Debug.Log("Cleared");
    }
    #endregion

}