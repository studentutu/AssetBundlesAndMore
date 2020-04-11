
using System.Collections.Generic;
using UnityEngine;

namespace KAUGamesLviv.Services.Bundles
{
    [CreateAssetMenu(fileName = "BundleDependencies", menuName = "AssetBundles/Bundle Dependencies", order = 2)]
    public class BundleDependencies : ScriptableObject
    {
        ///<summary> 
        /// Needed only for storing Data from editor, to take all depencency <see cref="ToDictionary()"/>
        ///</summary> 
        [SerializeField] private BundleData bundleDependinciesSerialized = new BundleData();
        [Space]
        [Tooltip("Only been Used when used with Own Names Mapping")]
        [SerializeField] private List<string> DepencenciesWithNoPrefixes = new List<string>();
        public Dictionary<string, List<string>> ToDictionary()
        {
            Dictionary<string, List<string>> bundleDependencies = new Dictionary<string, List<string>>();
            foreach (var item in bundleDependinciesSerialized.dictionaryitem)
            {
                bundleDependencies.Add(item.key, item.value);
            }
            return bundleDependencies;
        }

        public HashSet<string> ToHelpHashSetForOwnMappingDeps()
        {
            HashSet<string> result = new HashSet<string>();
            foreach (var item in DepencenciesWithNoPrefixes)
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
