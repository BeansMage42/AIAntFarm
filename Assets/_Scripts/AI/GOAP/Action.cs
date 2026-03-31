using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.AI.GOAP
{
    public abstract class BaseAction : MonoBehaviour
    {
        public string actionName = "Action";
        public float cost = 1;
        public float duration = 0;
        public WorldState[] PreConditions;
        public WorldState[] PostConditions;

        public Dictionary<string, int> dPreConditions;
        public Dictionary<string, int> dPostConditions;
    
        public WorldState agentBeliefs;
        public Locations location;
        public WorldStates beliefs;
    
        public bool inProgress = false;

        public BaseAction()
        {
            dPreConditions = new Dictionary<string, int>();
            dPostConditions = new Dictionary<string, int>();
        }

        public void Awake()
        {
            if (PreConditions != null)
                foreach (WorldState ws in PreConditions)
                {
                    dPreConditions.Add(ws.key, ws.value);
                }
        
            if (PostConditions != null)
                foreach (WorldState ws in PostConditions)
                {
                    dPostConditions.Add(ws.key, ws.value);
                }
        
            location = GetComponent<BaseAgent>().location;
            beliefs = GetComponent<BaseAgent>().beliefs;
        }

        public bool IsAchievable()
        {
            return true;
        }
    
        public bool IsAchievableGiven(Dictionary<string, int> conditions)
        {
            foreach (KeyValuePair<string, int> kvp in dPreConditions)
            {
                if (!conditions.ContainsKey(kvp.Key)) return false;
            }
            return true;
        }

        public abstract bool PrePerform();
        public abstract bool PostPerform();
        public abstract bool AchievedGoal();
        
    }
}
