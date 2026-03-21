using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class QuadTreeManager : MonoBehaviour
{

    public QuadTree tree;
    public int limit;
    public Transform maxPos;
    public Transform minPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTree();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateTree()
    {
        Vector3 center = new Vector3((maxPos.position.x + minPos.position.x)/2,maxPos.position.y,(maxPos.position.z + minPos.position.z)/2);
        //  Vector3 size = new Vector3()
        Bounds treeStartingBounds = new();
        treeStartingBounds.min = minPos.transform.position;
        treeStartingBounds.max = maxPos.transform.position;
        tree = new QuadTree(limit, treeStartingBounds);
    }

    private void OnDrawGizmos()
    {
        if (tree != null)
        {
            if (tree.nodes.Count != 0)
            {
                foreach (var node in tree.nodes)
                {

                    Bounds nodeBounds = node.GetNodeBounds();
                    /*                    Vector3 a = new Vector3(nodeBounds.min.x, nodeBounds.max.y, nodeBounds.max.z);
                                        Vector3 b = new Vector3(nodeBounds.max.x, nodeBounds.max.y, nodeBounds.max.z);
                                        Vector3 c = new Vector3(nodeBounds.max.x, nodeBounds.max.y, nodeBounds.min.z);
                                        Vector3 d = new Vector3(nodeBounds.min.x, nodeBounds.max.y, nodeBounds.min.z);
                                        Gizmos.DrawLine(a, b);
                                        Gizmos.DrawLine(b, c);
                                        Gizmos.DrawLine(c, d);
                                        Gizmos.DrawLine(d, a);*/
                   // Gizmos.color = new Color (1,0, (node._generation)/limit);
                    Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size);


                }
            }
        }
    }
}
