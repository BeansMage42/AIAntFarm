using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.AI.GOAP
{
    public abstract class BaseAction : MonoBehaviour
    {
        public string actionName = "Action";
        public float cost = 1;
        public GameObject target;
        public string targetTag;
        public float duration = 0;
        public WorldState[] PreConditions;
        public WorldState[] PostConditions;
        public NavMeshAgent agent;

        public Dictionary<string, int> dPreConditions;
        public Dictionary<string, int> dPostConditions;
    
        public WorldState agentBeliefs;
        public Locations location;
        public WorldStates beliefs;
    
        public bool isRunning = false;

        public BaseAction()
        {
            dPreConditions = new Dictionary<string, int>();
            dPostConditions = new Dictionary<string, int>();
        }

        public void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        
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
        
            location = this.GetComponent<BaseAgent>().location;
            beliefs = this.GetComponent<BaseAgent>().beliefs;
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
    }
}
