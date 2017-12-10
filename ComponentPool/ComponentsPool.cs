using UnityEngine;
using System.Collections.Generic;
using System;

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
    private PoolContainer _poolContainer;

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
                    item.transform.parent = null;
                    return item as T;
                }
            }

            var tempInstance = MonoBehaviour.Instantiate(prefab) as T;
            tempList.Add(tempInstance);
            return tempInstance;
        }
        else
        {
            if(!_poolContainer) 
            {
                var temp = new GameObject("PoolWrapper");
                _poolContainer = temp.AddComponent<PoolContainer>();
                if(_poolContainer)
                {
                    _poolContainer.OnDestroyWrapper += PoolContainerOnDestroy;
                    _poolContainer.transform.position = Vector3.zero;
                    _poolContainer.gameObject.SetActive(false);
                }
            }

            var component = prefab as Component;
            _pool.Add(component, new List<Component>());

            return GetClone<T>(prefab);
        }
    }

    private void PoolContainerOnDestroy ()
    {
        if(_poolContainer)
        {
            _poolContainer.OnDestroyWrapper -= PoolContainerOnDestroy;
            _pool.Clear();
        }
    }

    public void PutInPool<T>(T item) where T : Component
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(_poolContainer.transform);
    }

    #endregion
}


