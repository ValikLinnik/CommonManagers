﻿using System;
using System.Linq;
using Game.Data;
using UnityEngine;
using Injection;

public class StateComponent : InjectorBase<StateComponent>  
{
    #region INJECTED FIELDS

    [Inject]
    private StateManager _stateManager;

    #endregion

    #region SERIALIZE FIELDS

    [SerializeField, Tooltip("Active states")]
    private GameState[] _states;

    #endregion

    #region UNITY EVENTS

    protected virtual void Start()
    {
        if(_stateManager) _stateManager.OnStateChanged += OnStateChangedHandler;
        else throw new NullReferenceException("_stateManager is null.");
    }

    protected virtual void OnDestroy()
    {
        if(_stateManager) _stateManager.OnStateChanged -= OnStateChangedHandler;
    }

    #endregion

    #region implemented abstract members of StateListenerBase

    private void OnStateChangedHandler(GameState arg1, GameState arg2)
    {
        if (_states == null || _states.Length == 0) throw new Exception("States not set");

        var states = from i in _states where i == arg1 select i;
        bool condition = states.IsNullOrEmpty();
        gameObject.SetActive(!condition);
    }

    #endregion
}
