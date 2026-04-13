using System;
using _Scripts.AI.GOAP;
using UnityEngine;

public class ReleaseCollectors : BaseAction
{
    public override bool PrePerform()
    {
        World.Instance.SpawnSeeker();
        return true;
    }

    public override bool PostPerform()
    {
        World.Instance.SpawnSeeker();
        return true;
    }

    public override bool AchievedGoal()
    {
        return true;
    }
}
