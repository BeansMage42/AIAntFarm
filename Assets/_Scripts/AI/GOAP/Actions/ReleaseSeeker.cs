using _Scripts.AI.GOAP;
using UnityEditor;
using UnityEngine;

public class ReleaseSeeker : BaseAction
{
    public override bool PrePerform()
    {
        World.Instance.SpawnSeeker();
        this.inProgress = true;
        return true;
    }

    public override bool PostPerform()
    {
        return true;
    }

    public override bool AchievedGoal()
    {
        return true;
    }
}