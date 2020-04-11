using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(ManagerPool), true)]
public class EditorForSerializedPool : UnityEditor.Editor
{
    private ManagerPool CurrentManaer;

    private void OnEnable()
    {
        CurrentManaer = (ManagerPool)target;
    }
    public override void OnInspectorGUI()
    {

        if (GUILayout.Button(" Set Serialized Pools"))
        {
            ManagerPool managerPool = (ManagerPool)target;
            foreach (var itemPool in managerPool.allPools)
            {
                if (itemPool.parentPool != null)
                {
                    itemPool.ClearCachedEditor();
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(itemPool.parentPool);
                    var allObjectsForPools = itemPool.parentPool.GetComponentsInChildren<IPoolableClass>(true);
                    foreach (var itemPoolable in allObjectsForPools)
                    {
                        if (itemPool.MyPullType == itemPoolable.myPoolId)
                        {
                            itemPool.Despawn(itemPoolable);
                        }
                    }
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(itemPool.parentPool);
                }
                else
                {
                    UnityEngine.Debug.LogWarning(" Tranform Parent is null for  Pool : " + itemPool.MyPullType);
                }
            }
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(managerPool);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        DrawPropertiesExcluding(serializedObject, new string[] { "m_Script" });

        serializedObject.ApplyModifiedProperties();
    }

    private SerializedProperty GetPropertyByName<T>(SerializedObject toLookIn, string name = null)
    {
        System.Type typeToLookFor = typeof(T);
        // allLengthOfEnum = System.Enum.GetNames(typeof(KAU.Audio.SoundId)).Length;
        SerializedProperty serializedPropertyUserIds = null;
        foreach (System.Reflection.FieldInfo fi in target.GetType().GetFields(
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance))
        {
            // Debug.LogWarning(" All Fields Form : " + target.GetType().Name + " " +  fi.Name +  " type : " + PrettifyTypes.ST_PrettifyType(fi.FieldType) );
            if (fi.FieldType == typeToLookFor && (string.IsNullOrEmpty(name) || fi.Name == name))
            {
                return toLookIn.FindProperty(fi.Name);
            }
        }
        foreach (System.Reflection.PropertyInfo fi in target.GetType().GetProperties(
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance))
        {
            // Debug.LogWarning(" All Fields Form : " + target.GetType().Name + " " +  fi.Name +  " type : " + PrettifyTypes.ST_PrettifyType(fi.PropertyType));

            if (fi.PropertyType == typeToLookFor && (string.IsNullOrEmpty(name) || fi.Name == name))
            {
                return toLookIn.FindProperty(fi.Name);
            }
        }

        if (serializedPropertyUserIds == null)
        {
            Debug.LogError(string.Format("<Color=Red> Props Or Field is not found!  {0} </Color>", name));
        }
        return serializedPropertyUserIds;
    }

}
