using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scripts.Services
{
    [Serializable]
    public class ConverterJsonUtility : IConverter
    {
        public string SaveFileWithExtension => saveTo;

        [SerializeField] private string saveTo = "app.custom";
        [SerializeField] private TextAsset saveToAsset = null;

        void IServices.Init()
        {

        }

        public T FromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public string ToJson<T>(T toJson)
        {
            return JsonUtility.ToJson(toJson);
        }

        public T LoadData<T>(string fileWithExtension)
        {
            var path = Path.Combine(Application.persistentDataPath, fileWithExtension);
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        public void SaveFile(string fileWithExtension, object data)
        {
            var path = Path.Combine(Application.persistentDataPath, fileWithExtension);
            Debug.LogWarning("path : " + path);
            File.WriteAllText(path, JsonUtility.ToJson(data));
        }

#if UNITY_EDITOR
        public void WriteToLocalTextAsset(string fullString, TextAsset saveTo = null)
        {
            if (saveTo == null)
            {
                saveTo = saveToAsset;
            }
            string path = UnityEditor.AssetDatabase.GetAssetPath(saveTo);
            int first = path.IndexOf('/');
            if (first == -1)
            {
                first = path.IndexOf('\\');
            }
            path.Replace('/', '\\');
            path = path.Substring(first + 1);
            path = Path.Combine(Application.dataPath, path);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Please specify the Asset to write into");
                return;
            }

            try
            {
                // At runtime if we will write to the Assets Folder - will give us exceptions,
                // but will still successfully write to the asset
                File.WriteAllText(path, fullString);
            }
            catch (System.Exception)
            {
            }
            finally
            {
                UnityEditor.EditorUtility.SetDirty(saveTo);
                UnityEditor.AssetDatabase.SaveAssets();
            }
        }

        public string ReadAllFromTExtAsset(TextAsset loadFrom = null)
        {

            if (loadFrom == null)
            {
                loadFrom = saveToAsset;
            }

            string path = UnityEditor.AssetDatabase.GetAssetPath(loadFrom);
            int first = path.IndexOf('/');
            if (first == -1)
            {
                first = path.IndexOf('\\');
            }
            path.Replace('/', '\\');
            path = path.Substring(first + 1);
            path = Path.Combine(Application.dataPath, path);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Please specify the Asset to write into");
                return null;
            }
            return File.ReadAllText(path);
        }
#endif
    }
}
