using UnityEngine;

public class FSM_StateMachine : FSM_AbstractState
{
    public FSM_AbstractState CurrentState { get; private set; } = null;

    public void JumpToState(FSM_AbstractState newState)
    {
        if (CurrentState == newState) return;

        if (CurrentState != null) CurrentState.OnExitState();

        CurrentState = newState;
        if(CurrentState != null) CurrentState.OnEnterState();
    }

    public override void OnEnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void Update()
    {

        if (CurrentState == null) return;

        FSM_Transition nextTransition = CurrentState.PollTransitions();

        if (nextTransition != null)
        {
            CurrentState.OnExitState();
            nextTransition.ExecuteTransition();
            CurrentState = nextTransition.NextState;
        }
        CurrentState.Update();
    }
}
