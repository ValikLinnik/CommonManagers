using UnityEngine;
using System.Collections.Generic;

public static class PoolExtentions
{
    public static T GetInstance<T>(this T obj) where T : Component
    {
        return ComponentPool.Instance.GetClone<T>(obj);
    }

    public static void PutInPool<T>(this T obj) where T : Component
    {
        obj.gameObject.SetActive(false);
    }
}

public class ComponentPool
{
    #region SINGLETONE PART

    private ComponentPool()
    {

    }

    private static ComponentPool _instance;

    public static ComponentPool Instance
    {
        get
        {
            if (_instance == null) _instance = new ComponentPool();
            return _instance;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private Dictionary<Component, List<Component>> _pool = new Dictionary<Component, List<Component>>();

    #endregion

    #region PUBLIC METHODS

    public T GetClone<T>(T prefab) where T : Component
    {
        if (_pool.ContainsKey(prefab))
        {
            var tempList = _pool[prefab];

            foreach (var item in tempList)
            {
                if (!item) continue;
                if (!item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(true);
                    return item as T;
                }
            }

            var tempInstance = Object.Instantiate(prefab) as T;
            tempList.Add(tempInstance);
            return tempInstance;
        }
        else
        {
            _pool.Add(prefab as Component, new List<Component>());
            return GetClone<T>(prefab);
        }
    }

    #endregion
}


