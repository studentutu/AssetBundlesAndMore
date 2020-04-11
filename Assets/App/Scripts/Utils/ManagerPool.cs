using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class IPoolableClass : MonoBehaviour, IPoolable
{
    public int myPoolId = 0;
    public abstract void OnDespawn();
    public abstract void OnSpawn();
}
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}
public class ManagerPool : MonoBehaviour
{
    [SerializeField] public List<IPool> allPools = new List<IPool>();
    private Dictionary<int, IPool> m_pools = null;
    private Dictionary<int, IPool> pools
    {
        get
        {
            if (m_pools == null)
            {
                m_pools = new Dictionary<int, IPool>();
                foreach (var item in allPools)
                {
                    pools.Add(item.MyPullType, item);
                }
            }
            return m_pools;
        }
    }

    public IPool AddPooledDict<T>(T SomeonTOTakeIdFrom)
        where T : IPoolableClass
    {
        IPool pool;
        int id = SomeonTOTakeIdFrom.myPoolId;
        if (pools.TryGetValue(id, out pool) == false)
        {
            pool = new IPool();
            pools.Add(id, pool);
            var poolGO = new GameObject("Pool:" + id);
            poolGO.transform.SetParent(this.transform);
            pool.SetParent(poolGO.transform);
        }
        return pool;
    }

    public void PopulatePool<T>(T prefab, Vector3 worldPos, int AddAmount)
        where T : IPoolableClass
    {
        pools[prefab.myPoolId].PopulateWith(prefab, worldPos, AddAmount);
    }


    public T Spawn<T>(T prefab, Vector3 position = default(Vector3),
        Quaternion rotation = default(Quaternion),
        Transform parent = null)
        where T : IPoolableClass
    {
        CheckPool<T>(prefab);
        return pools[prefab.myPoolId].Spawn(prefab, position, rotation, parent) as T;
    }
    private void CheckPool<T>(T obj) where T : IPoolableClass
    {
        if (!pools.ContainsKey(obj.myPoolId))
            AddPooledDict(obj);
    }

    public void Despawn<T>(T obj, bool Reparent = true)
        where T : IPoolableClass
    {
        CheckPool<T>(obj);
        pools[obj.myPoolId].Despawn(obj, Reparent);
    }
    public int GetNumberOfPoolelems<T>(T obj) where T : IPoolableClass
    {
        CheckPool<T>(obj);
        return pools[obj.myPoolId].GetNumberOfPoolelems();
    }

}


[System.Serializable]
public class IPool : IDisposable
{
    #pragma warning disable
    [SerializeField] private int CurrentlyUsedFromPool = 0;
    #pragma warning restore


    [SerializeField] public int MyPullType = 0;
    [SerializeField] public Transform parentPool;
    [SerializeField] private List<IPoolableClass> cachedObjects = new List<IPoolableClass>();
    public int GetNumberOfPoolelems()
    {
        return cachedObjects.Count;
    }
    public void ClearCachedEditor()
    {
        cachedObjects.Clear();
    }
    public void PopulateWith(IPoolableClass prefab, Vector3 WorldPositionInit, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Spawn(prefab, WorldPositionInit);
        }

    }

    public void SetParent(Transform parent)
    {
        parentPool = parent;
    }

    public IPoolableClass Spawn(IPoolableClass prefab, Vector3 WorldPositionInit, Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        IPoolableClass result = null;
        if (cachedObjects.Count > 0)
        {
            result = cachedObjects[0];
            cachedObjects.RemoveAt(0);
        }
        else
        {
            result = Populate(prefab, WorldPositionInit, rotation, parentPool);
        }
        if (parent != null)
            result.transform.SetParent(parent, false);

        result.transform.position = WorldPositionInit;
        // result.transform.gameObject.SetActive(true);

#if UNITY_EDITOR
        CurrentlyUsedFromPool ++;
        result.gameObject.name = string.Format(nameOfPrefab, prefab.name,CurrentlyUsedFromPool ); 
#endif
        result.OnSpawn();
        return result;
    }
    private const string nameOfPrefab = "{0} ({1})";


    public void Despawn(IPoolableClass go, bool Reparent = true)
    {
        cachedObjects.Add(go);
#if UNITY_EDITOR
        CurrentlyUsedFromPool --;
        if(CurrentlyUsedFromPool < 0 ) CurrentlyUsedFromPool = 0;
#endif
        go.OnDespawn();
        // go.gameObject.SetActive(false);
        if (Reparent)
            go.transform.SetParent(parentPool, false);
    }


    private IPoolableClass Populate(IPoolableClass prefab, Vector3 position = default(Vector3),
        Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        IPoolableClass  go = GameObject.Instantiate(prefab, position, rotation, parent);
        go.transform.position = position;
        return go;
    }

    public void Dispose()
    {
        cachedObjects.Clear();
        parentPool = null;
    }
}
