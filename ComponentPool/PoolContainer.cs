using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolContainer : MonoBehaviour 
{
    public event Action OnDestroyWrapper;

    private void OnDestroyWrapperHandler()
    {
        if (OnDestroyWrapper != null)
            OnDestroyWrapper();
    }

    private void OnDestroy()
    {
        OnDestroyWrapperHandler();
    }
}
