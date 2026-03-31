using System.Collections.Generic;
using _Scripts.AI.GOAP;
using UnityEngine;

public class Planner {

    public Queue<BaseAction> plan(List<BaseAction> actions, Dictionary<string, int> goal, WorldStates beliefStates) {

        List<BaseAction> usableActions = new List<BaseAction>();
        
        foreach (BaseAction a in actions) {

            if (a.IsAchievable()) {

                usableActions.Add(a);
            }
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0.0f, World.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);
        
        bool success = BuildGraph(start, leaves, usableActions, goal);
        
        if (!success) {
            
            return null;
        }
        
        Node cheapest = null;
        foreach (Node leaf in leaves) {

            if (cheapest == null) {

                cheapest = leaf;
            } else if (leaf.cost < cheapest.cost) {

                cheapest = leaf;
            }
        }
        List<BaseAction> result = new List<BaseAction>();
        Node n = cheapest;

        while (n != null) {

            if (n.action) {

                result.Insert(0, n.action);
            }

            n = n.parent;
        }
        
        Queue<BaseAction> queue = new Queue<BaseAction>();

        foreach (BaseAction a in result) {

            queue.Enqueue(a);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<BaseAction> usableActions, Dictionary<string, int> goal) {

        bool foundPath = false;
        
        foreach (BaseAction action in usableActions) {
            
            if (action.IsAchievableGiven(parent.state)) {
                
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                
                foreach (KeyValuePair<string, int> eff in action.dPostConditions) {

                    if (!currentState.ContainsKey(eff.Key)) {

                        currentState.Add(eff.Key, eff.Value);
                    }
                }
                
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if (GoalAchieved(goal, currentState)) {

                    leaves.Add(node);
                    foundPath = true;
                } else {
                    List<BaseAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);

                    if (found) {

                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }
    
    private List<BaseAction> ActionSubset(List<BaseAction> actions, BaseAction removeMe) {

        List<BaseAction> subset = new List<BaseAction>();

        foreach (BaseAction a in actions) {

            if (!a.Equals(removeMe)) {

                subset.Add(a);
            }
        }
        return subset;
    }
    
    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state) {

        foreach (KeyValuePair<string, int> g in goal) {

            if (!state.ContainsKey(g.Key)) {

                return false;
            }
        }
        return true;
    }
}

public class Node {
    
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public BaseAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allStates, BaseAction action) {

        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allStates);
        this.action = action;
    }

    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates, BaseAction action) {

        this.parent = parent;
        this.cost = cost;
        state = new Dictionary<string, int>(allStates);
        
        foreach (KeyValuePair<string, int> b in beliefStates) {

            if (!state.ContainsKey(b.Key)) {

                state.Add(b.Key, b.Value);
            }
        }
        this.action = action;
    }
}
