using UnityEngine;
using System.Collections.Generic;
public abstract class FSM_AbstractState
{
    private List<FSM_Transition> transitions = new List<FSM_Transition>();

    public void AddTransition(FSM_Transition transition)
    {
        Debug.Log("add transition");
        transitions.Add(transition);
    }

    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract void Update();
    public FSM_Transition PollTransitions()
    {
        for (int i = 0; i < transitions.Count; i++)
        {
            if (transitions[i].CanTransition())
            {
                return transitions[i];
            }
        }
        return null;
    }

}
