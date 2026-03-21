using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class QuadTreeManager : MonoBehaviour
{
    [SerializeReference]
    public QuadTree tree;
    public int limit;
    public Transform maxPos;
    public Transform minPos;

    public Transform test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTree();
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (TreeContainsPoint(test.transform.position, out Quad newQuad))
            {
                Debug.Log("quad at: " + newQuad.GetNodeBounds().center + " contains point");
                Debug.DrawLine(newQuad.GetNodeBounds().center, newQuad.GetNodeBounds().center + Vector3.up * 10, Color.green, 0.5f);
            }
            else
            {
                Debug.Log("tree did not contain point");
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (TreeContainsBounds(test.gameObject.GetComponent<Collider>().bounds, out Quad[] newQuad))
            {
                foreach (Quad quad in newQuad)
                {
                    Debug.Log("quad at: " + quad.GetNodeBounds().center + " contains point");
                    Debug.DrawLine(quad.GetNodeBounds().center, quad.GetNodeBounds().center + Vector3.up * 10, Color.green, 0.5f);
                }
               
            }
            else
            {
                Debug.Log("tree did not contain point");
            }
        }
    }

    public void GenerateTree()
    {
        Vector3 center = new Vector3((maxPos.position.x + minPos.position.x)/2,0,(maxPos.position.z + minPos.position.z)/2);
        //  Vector3 size = new Vector3()
        Bounds treeStartingBounds = new();
        treeStartingBounds.min = minPos.transform.position;
        treeStartingBounds.max = maxPos.transform.position;
        treeStartingBounds.size = new Vector3(treeStartingBounds.size.x,10,treeStartingBounds.size.z);
        tree = new QuadTree(limit, treeStartingBounds);
    }
    public bool TreeContainsPoint(Vector3 point, out Quad foundQuad)
    {
        point.y = tree.nodes[0].GetNodeBounds().center.y;
        Debug.Log("testing point" + point);
        foundQuad = null;
        return tree.TreeContainsPoint(point, out foundQuad);
    }
    public bool TreeContainsBounds(Bounds bounds, out Quad[] intersectingQuads)
    {
        if(tree.TreeContainsBounds(bounds, out intersectingQuads))return true;
        return false;

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
