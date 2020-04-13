using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;
using Services.Bundles.Services.Bundles.Editor;

namespace Services.Bundles
{
    [CustomEditor(typeof(BundleDependencies), true)]
    public class BundleDependenciesEditor : CustomExtendedEditor
    {
        BundleDependencies _bundleDep;
        IGetNames asIGetNames;
        SerializedProperty _bundleDependenciesSerialized;
        ReorderableList _listOfDependencies;
        public bool showProjectModule = false;
        public bool showOwnMappingModule = false;
        private int indexFocused = -1;
        private float HeightForActive = 120;

        private Dictionary<string, SerializedProperty> _dictionaryProperties;

        public void OnEnable()
        {
            _bundleDep = target as BundleDependencies;
            asIGetNames = _bundleDep;
            var allProps = new List<string>();
            _dictionaryProperties = null;
            Dictionary<string, SerializedProperty> allProperties = new Dictionary<string, SerializedProperty>();
            var myObj = _bundleDep;
            if (myObj != null)
            {
                allProps.AddRange
                    (myObj.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(fieldInfo => fieldInfo.Name));
            }
            foreach (var item in allProps)
            {
                var something = serializedObject.FindProperty(item);
                if (!allProperties.ContainsKey(item) && something != null)
                {
                    allProperties.Add(item, something);
                }
                if (something != null && item.Contains(asIGetNames.GetBundleDepPropName()))
                {
                    _bundleDependenciesSerialized = something;
                }

            }
            _dictionaryProperties = allProperties;
            _listOfDependencies = new ReorderableList(serializedObject, _bundleDependenciesSerialized.FindPropertyRelative("dictionaryitem"), true, true, true, true);
            _listOfDependencies.drawElementCallback = DrawProductsElement;
            _listOfDependencies.drawHeaderCallback = DrawProductsHeader;
            _listOfDependencies.elementHeightCallback = CalculateHeight;
        }
        private float CalculateHeight(int index)
        {
            if (indexFocused != index)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return HeightForActive;
        }
        private void DrawProductsElement(Rect rect, int i, bool isActive, bool isFocused)
        {
            var element = _bundleDependenciesSerialized.FindPropertyRelative("dictionaryitem").GetArrayElementAtIndex(i);
            rect.y += 2;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, 120), (i + 1) + ". Key");
            EditorGUI.PropertyField(new Rect(rect.x + 51, rect.y, 180, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("key"), GUIContent.none);
            if (isActive && isFocused)
            {
                indexFocused = i;
            }
            if (indexFocused == i)
            {
                var anotherSerializedPropertyList = element.FindPropertyRelative("value");
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 1, Screen.width, Screen.height), anotherSerializedPropertyList, true);
                if (anotherSerializedPropertyList.isExpanded)
                {
                    HeightForActive = (anotherSerializedPropertyList.arraySize + 3) * (EditorGUIUtility.singleLineHeight + 2.1f);
                }
                else
                {
                    HeightForActive = 3 * EditorGUIUtility.singleLineHeight;
                }
                anotherSerializedPropertyList.serializedObject.ApplyModifiedProperties();
            }
        }
        private void DrawProductsHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Dictionary Entries");
        }
        public override void OnInspectorGUI()
        {

            showProjectModule = GUILayout.Toggle(showProjectModule, "Dictionary Entries", styles.Header, GUILayout.Height(20), GUILayout.ExpandWidth(true));
            if (showProjectModule) SerializedDictionary();

            showOwnMappingModule = GUILayout.Toggle(showOwnMappingModule, "Own Mapping", styles.Header, GUILayout.Height(20), GUILayout.ExpandWidth(true));
            if (showOwnMappingModule) DisplayOwnMapping();

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawDefaultProps(SerializedProperty serialObjectInner)
        {
            EditorGUILayout.PropertyField(serialObjectInner, true);
        }

        private void SerializedDictionary()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            EditorGUILayout.BeginVertical();

            _listOfDependencies.DoLayoutList();

            EditorGUILayout.EndVertical();
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayOwnMapping()
        {
            foreach (var item in _dictionaryProperties.Keys)
            {
                if (item.Equals(asIGetNames.GetBundleDepPropName()))
                {
                    continue;
                }
                else
                {
                    DrawDefaultProps(_dictionaryProperties[item]);
                }
            }
        }
    }
}