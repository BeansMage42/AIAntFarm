using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.AI.GOAP
{
    public class BaseAgent : MonoBehaviour {
    
        public List<BaseAction> actions = new ();
        public Dictionary<SubGoal, int> goals = new ();
        public WorldStates beliefs = new ();
        public float UpdateDelay = 5f;
    
        Planner planner;
        Queue<BaseAction> actionQueue;
        public BaseAction currentAction;
        SubGoal currentGoal;
        private float timer;
    
        public void Start() {

            BaseAction[] acts = GetComponents<BaseAction>();
            foreach (BaseAction a in acts) {
                actions.Add(a);
            }
        }

        bool invoked;
        public void CompleteAction() {

            currentAction.inProgress = false;
            currentAction.PostPerform();
            currentAction = null;
            invoked = false;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > UpdateDelay)
            {
                timer = 0;
                DailyUpdate();
                print("Hell9");
            }
        }
        void DailyUpdate() {
        
            if (currentAction != null && currentAction.inProgress) {
                
                if (currentAction.AchievedGoal()) {

                    if (!invoked) {
                        Invoke("CompleteAction", currentAction.duration);
                        invoked = true;
                    }
                }
                return;
            }
        
            if (planner == null || actionQueue == null) {
                planner = new Planner();
            
                var sortedGoals = from entry in goals orderby entry.Value descending select entry;

                foreach (KeyValuePair<SubGoal, int> sg in sortedGoals) {

                    actionQueue = planner.plan(actions, sg.Key.sGoals, beliefs);
                    if (actionQueue != null) {
                        currentGoal = sg.Key;
                        break;
                    }
                }
            }
        
            if (actionQueue != null && actionQueue.Count == 0) {
            
                if (currentGoal.remove) {
                
                    goals.Remove(currentGoal);
                }
    
                planner = null;
            }
        
            if (actionQueue != null && actionQueue.Count > 0) {
            
                currentAction = actionQueue.Dequeue();

                if (currentAction.PrePerform()) {
                    
                } else {
                
                    actionQueue = null;
                }
            }
        }
    }

    public class SubGoal {
    
        public Dictionary<string, int> sGoals;

        public bool remove;
    
        public SubGoal(string s, int i, bool r) {

            sGoals = new Dictionary<string, int>();
            sGoals.Add(s, i);
            remove = r;
        }
    }
}