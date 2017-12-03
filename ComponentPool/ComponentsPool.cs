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
        ComponentPool.Instance.PutInPool(obj);
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
    private Dictionary<Component, Transform> _scenePool = new Dictionary<Component, Transform>();
    private GameObject _poolWrapper;

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
            if(!_poolWrapper) 
            {
                _poolWrapper = new GameObject("PoolWrapper");
                _poolWrapper.transform.position = Vector3.zero;
                _poolWrapper.SetActive(false);
            }

            var component = prefab as Component;
            _pool.Add(component, new List<Component>());
            var temp = new GameObject(component.GetType().ToString());
            temp.transform.parent = _poolWrapper.transform;
            _scenePool.Add(component, temp.transform);

            return GetClone<T>(prefab);
        }
    }

    public void PutInPool<T>(T item) where T : Component
    {
        item.gameObject.SetActive(false);
        var parent = _scenePool[item];
        item.transform.parent = parent;
    }

    #endregion
}


