using System.Collections.Generic;
using System.Linq;
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



    public static QuadTreeManager Instance;

    private List<QuadTreeObject> objects = new();
    public bool ShowQuads = false;
    private void Awake()
    {
        if (Instance != null)
        {
            if(Instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateTree();
        objects.AddRange( FindObjectsByType<QuadTreeObject>(FindObjectsInactive.Exclude,FindObjectsSortMode.None));
        tree.objects = objects;
        Debug.Log("found objects" +  objects.Count);    
        tree.GenerateTree();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (TreeContainsPoint(objects[0].transform.position, out Quad newQuad))
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
            if (TreeContainsBounds(objects[0].bounds, out Quad[] newQuad))
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
        if (tree == null) { foundQuad = null; return false; }
        point.y = tree.nodes[0].GetNodeBounds().center.y;
        Debug.Log("testing point" + point);
        foundQuad = null;
        return tree.TreeContainsPoint(point, out foundQuad);
    }
    public bool TreeContainsBounds(Bounds bounds, out Quad[] intersectingQuads)
    {
        if (tree == null) { intersectingQuads = null; return false; }
        if (tree.TreeContainsBounds(bounds, out intersectingQuads))return true;
        return false;

    }

    public void AddObjectToTree(QuadTreeObject quadTreeObject)
    {
        if(objects.Contains(quadTreeObject)) return;
        objects.Add(quadTreeObject);
        tree.objects = objects;
        tree.ClearTree();
        tree.GenerateTree();
    }
    public void RemoveObjectFromTree(QuadTreeObject quadTreeObject)
    {
        if (!objects.Contains(quadTreeObject)) return;
        objects.Remove(quadTreeObject);
        tree.objects = objects;
        tree.ClearTree();
        tree.GenerateTree();

    }
    private void OnDrawGizmos()
    {
        if (ShowQuads)
        {
            if (tree != null)
            {
                if (tree.nodes.Count != 0)
                {
                    foreach (var node in tree.nodes)
                    {

                        Bounds nodeBounds = node.GetNodeBounds();
                        if (node._scents.Count > 0)
                        {
                            Gizmos.color = new Color(0, 0, node._scents.First().Value.Item2);
                            Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size + Vector3.up * 2);

                        }
                        else
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size);
                        }


                    }
                }
            }
        }
    }
}
