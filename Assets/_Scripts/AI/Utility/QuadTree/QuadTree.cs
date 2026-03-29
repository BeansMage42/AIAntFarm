using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class QuadTree
{
   // [SerializeReference]
    public List<Quad> nodes = new List<Quad>();
    public int limit;
    public Bounds rootBounds;
    public List<QuadTreeObject> objects = new();
    public QuadTree(int _l,Bounds _r)
    {
        limit = _l;
        rootBounds = _r;
        //AddRoot(rootBounds);
        //GenerateTree();
    }
    
    public void GenerateTree()
    {
        // nodes[0].Subdivide();
        Debug.Log("generating tree");
        AddRoot(rootBounds);
        foreach (var obj in objects)
        {
            nodes[0].AddObject(obj);
        }
        nodes.AddRange(nodes[0].GetDescendants());
    }
    private void AddRoot(Bounds firstNode)
    {
        nodes.Add(new Quad(0,null,firstNode,limit));
    }

    public bool TreeContainsPoint(Vector3 point, out Quad q)
    {
        
        if (nodes[0].QuadContainsPoint(point, out q)) return true;
       // Debug.Log("root node did not contain");
        q = null;
        return false;
    }
    public bool TreeContainsBounds(Bounds bounds, out Quad[] intersectingQuads)
    {
        
        if (nodes[0].QuadIntersectsBounds(bounds, out intersectingQuads)) return true;
        return false;

    }
    
    public void ClearTree() 
    {
        Debug.Log("clearing tree");
        nodes.Clear();
    }
}
