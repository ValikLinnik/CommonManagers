using System;
using Game.Data;
using Injection;

public class StateManager : InjectorBase<StateManager>
{
    #region EVENTS AND HANDLERS

    public event Action<GameState,GameState> OnStateChanged;

    private void OnStateChangedHandler(GameState current, GameState previous)
    {
        if (OnStateChanged != null) { OnStateChanged(current, previous);}
    }

    #endregion

    #region PUBLIC METHODS

    public GameState CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    #endregion

    #region PRIVATE FIELDS

    private GameState _currentState;
    private GameState _previousState;

    #endregion

    #region PUBLIC METHODS

    public void SetState(GameState newState)
    {
        _previousState = _currentState;
        _currentState = newState;
        OnStateChangedHandler(_currentState, _previousState);
    }

    #endregion
}
