using System;
using UnityEngine;

public class FSM_Transition
{
    public Action OnTransition;
    public Func<bool> Condition;
    public FSM_AbstractState NextState {  get; private set; }

    public FSM_Transition( FSM_AbstractState nextState, Action onTransition, Func<bool> condition)
    {
        OnTransition = onTransition;
        Condition = condition;
        NextState = nextState;
    }

    public void ExecuteTransition()
    {
        OnTransition?.Invoke();
        NextState.OnEnterState();
    }

    public bool CanTransition()
    {
        if(Condition ==  null)return false;
        return Condition();
    }
}
