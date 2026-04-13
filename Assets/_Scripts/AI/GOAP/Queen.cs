using _Scripts.AI.GOAP;
using UnityEngine;

public class Queen : BaseAgent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        
        SubGoal s1 = new SubGoal("GetFood", 1, false);
        goals.Add(s1, 3);
        
    }

}
