using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class QuadTree
{
    public List<Quad> nodes = new List<Quad>();
    public int limit;
    public Bounds rootBounds;
    public QuadTree(int _l,Bounds _r)
    {
        limit = _l;
        rootBounds = _r;
        AddRoot(rootBounds);
        GenerateTree();
    }
    
    public void GenerateTree()
    {
        nodes[0].Subdivide();
        nodes.AddRange(nodes[0].GetDescendants());
    }
    private void AddRoot(Bounds firstNode)
    {
        nodes.Add(new Quad(0,null,firstNode,limit));
    }

    public bool TreeContainsPoint(Vector3 point, out Quad q)
    {
        if (nodes[0].QuadContainsPoint(point, out q)) return true;
        q = null;
        return false;
    }
}
