
using System.Collections.Generic;
using UnityEngine;

namespace Services.Bundles
{
    namespace Services.Bundles.Editor
    {
        public interface IGetNames
        {
            string GetBundleDepPropName();
        }
    }

    [CreateAssetMenu(fileName = "BundleDependencies", menuName = "AssetBundles/Bundle Dependencies", order = 2)]
    public class BundleDependencies : ScriptableObject
    , Services.Bundles.Editor.IGetNames
    {
        ///<summary> 
        /// Needed only for storing Data from editor, to take all depencency <see cref="ToDictionary()"/>
        ///</summary> 
        [SerializeField] private BundleData bundleDependenciesSerialized = new BundleData();
        [Space]
        [Tooltip("Only been Used when used with Own Names Mapping")]
        [SerializeField] private List<string> DependenciesWithNoPrefixes = new List<string>();
        public Dictionary<string, List<string>> ToDictionary()
        {
            Dictionary<string, List<string>> bundleDependencies = new Dictionary<string, List<string>>();
            foreach (var item in bundleDependenciesSerialized.dictionaryitem)
            {
                bundleDependencies.Add(item.key, item.value);
            }
            return bundleDependencies;
        }

        string Services.Bundles.Editor.IGetNames.GetBundleDepPropName()
        {
            return nameof(bundleDependenciesSerialized);
        }

        public HashSet<string> ToHelpHashSetForOwnMappingDeps()
        {
            HashSet<string> result = new HashSet<string>();
            foreach (var item in DependenciesWithNoPrefixes)
            {
                result.Add(item);
            }
            return result;
        }

        [System.Serializable]
        public class DictionaryItem
        {
            public string key;
            public List<string> value = new List<string>();
        }

        [System.Serializable]
        public class BundleData
        {
            public List<DictionaryItem> dictionaryitem;
        }
    }
}
