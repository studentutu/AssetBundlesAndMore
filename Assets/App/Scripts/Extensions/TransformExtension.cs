using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public struct TransformLocalStruct
{
    public Transform parent;
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
    public bool active;
}

[System.Serializable]
public struct TransformGlobalStruct
{
    public Vector3 Position;
    public Vector3 EulerAngles;
    public Vector3 Scale;
    public bool active;
}

public static class TransformExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FindAncestorOfType<T>(this Transform t, int deep = 100) where T : Component
    {
        Transform temporal = t;
        T comp = temporal.GetComponent<T>();
        while (comp == null && temporal.parent != null && deep > 0)
        {
            temporal = temporal.parent;
            comp = temporal.GetComponent<T>();
            deep--;
        }

        return comp;
    }
    private static Transform helperObject = null;
    /// <summary>
    ///  Global slcae is a lossy scale!
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        if (helperObject == null)
        {
            helperObject = new GameObject("Helper Transform").transform;
            helperObject.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }
        helperObject.localScale = globalScale;
        helperObject.parent = transform.parent;
        transform.localScale = helperObject.localScale;
        helperObject.parent = null;
    }

    public static Transform AddChild(this Transform transform, string name = "GameObject")
    {
        Transform child = new GameObject(name).transform;
        child.parent = transform;
        child.localEulerAngles = Vector3.zero;
        child.localPosition = Vector3.zero;
        child.localScale = new Vector3(1, 1, 1);
        child.position = new Vector3(0, 0, 0);
        return child;
    }

    public static Transform AddChild(this Transform transform, GameObject origin, string name = "GameObject")
    {
        Transform child = GameObject.Instantiate(origin, transform).transform;
        child.name = name;
        return child;
    }

    public static void RemoveAllChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public static TransformLocalStruct ToLocalStruct(this Transform transform)
    {
        return new TransformLocalStruct
        {
            localPosition = transform.localPosition,
            localEulerAngles = transform.localEulerAngles,
            localScale = transform.localScale,
            active = transform.gameObject.activeSelf,
            parent = transform.parent
        };
    }
    public static TransformGlobalStruct ToGlobalStruct(this Transform transform)
    {
        return new TransformGlobalStruct
        {
            Position = transform.localPosition,
            EulerAngles = transform.localEulerAngles,
            Scale = transform.localScale,
            active = transform.gameObject.activeSelf,
        };
    }

    public static void FromGlobalStruct(this Transform transform, TransformGlobalStruct data, bool ignoreActive = true)
    {
        // transfrom from Local to World
        transform.position = data.Position;
        transform.eulerAngles = data.EulerAngles;
        transform.SetGlobalScale(data.Scale);
        if (!ignoreActive)
        {
            transform.gameObject.SetActive(data.active);
        }
    }

    /// <summary> Convert struct to local position for transform </summary>
    public static void FromLocalStructToLocal(this Transform transform, TransformLocalStruct data, bool ignoreActive = true)
    {
        // transfrom from Local to World
        transform.localPosition = data.localPosition;
        transform.localEulerAngles = data.localEulerAngles;
        transform.localScale = data.localScale;
        if (!ignoreActive)
        {
            transform.gameObject.SetActive(data.active);
        }
    }

    public static void FromLocalStructToWorld(this Transform transform, TransformLocalStruct data, bool ignoreActive = true)
    {
        // transfrom from Local to World + parent Position!
        transform.position = (data.parent.localToWorldMatrix * data.localPosition);
        transform.position += data.parent.position;
        transform.localEulerAngles = data.localEulerAngles;
        transform.localScale = data.localScale;
        if (!ignoreActive)
        {
            transform.gameObject.SetActive(data.active);
        }
    }

    public static string WholePath(this Transform current)
    {
        string s = "" + current.name;
        if (current.parent != null)
        {
            s += "/" + WholePath(current.parent);
        }
        return Reverse(s);
    }

    private static string Reverse(string s)
    {
        string[] charArray = s.Split('/');
        System.Array.Reverse(charArray);
        string fs = charArray[0];
        for (int i = 1; i < charArray.Length; i++)
        {
            fs += "/" + charArray[i];
        }
        return fs;
    }


}
