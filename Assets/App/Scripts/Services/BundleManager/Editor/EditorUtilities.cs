#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KAUGamesLviv.Services.Bundles;

public class EditorUtilities : MonoBehaviour
{

    #region Tools_for_tests
    [UnityEditor.MenuItem("Assets/ClearCachedFiles")]
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
    [UnityEditor.MenuItem("Assets/ClearPrefs")]
    public static void ClearPrefs()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        UnityEngine.Debug.Log("Cleared");
    }
    #endregion

    [MenuItem("Tools/BundleManager/MakeBundleManagerInstance")]
    public static void CreateBUndleManagerInstance()
    {
        BundleManagerInstance customPlayerPrefsStatic = ScriptableObject.CreateInstance<BundleManagerInstance>();
        AssetDatabase.CreateAsset(customPlayerPrefsStatic, "Assets/Resources/BundleManagerInstance.asset"); // always .asset for arbitrary assets!
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = customPlayerPrefsStatic;

    }

}
#endif
