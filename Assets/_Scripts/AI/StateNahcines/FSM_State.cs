using System;
using UnityEngine;

public class FSM_State : FSM_AbstractState
{
    public Action OnEnterStateAction;
    public Action OnExitStateAction;
    public Action OnProcessStateAction;

    public FSM_State(Action onEnterStateAction,  Action onProcessStateAction, Action onExitStateAction)
    {
        OnEnterStateAction = onEnterStateAction;
        OnExitStateAction = onExitStateAction;
        OnProcessStateAction = onProcessStateAction;
    }

    public override void OnEnterState()
    {
        Debug.Log("state entered");
       OnEnterStateAction?.Invoke();
    }

    public override void OnExitState()
    {
        OnExitStateAction?.Invoke();
    }

    public override void Update()
    {
        OnProcessStateAction?.Invoke();
    }
}
