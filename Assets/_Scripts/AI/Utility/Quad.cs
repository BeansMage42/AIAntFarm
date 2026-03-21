using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Quad
{
    //Vector3 _center;
    public Bounds _bounds;
    public int _generation;
    int _limit;
    Quad _parent;
    Quad[] children = null;

    public Quad(int _g, Quad _par, Bounds _b, int _l)
    {
        _parent = _par;
        _generation = _g;
        _bounds = _b;
        _limit = _l;
    }

    public void Subdivide()
    {
        if(_generation >=  _limit) return;
        children = new Quad[4];
        float length = _bounds.size.x/4;
        float height = _bounds.size.z/4;
        Vector3 childSize = new Vector3(length, 0, height)*2;
        children[0] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(-length, 0, -height), childSize), _limit);
        children[1] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(length, 0, -height), childSize), _limit);
        children[2] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(length, 0, height), childSize), _limit);
        children[3] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(-length, 0, height), childSize), _limit);
        if (_generation + 1 < _limit)
        {
            for (int i = 0; i < 4; i++)
            {
                children[i].Subdivide();
            }
        }
    }

    public Quad[] GetDescendants()
    {
        List<Quad> descendants = new List<Quad>();

        if(children == null ||  children.Length == 0)
        {
            return descendants.ToArray();
        }
        else
        {
            descendants.AddRange(children);
            foreach(var child in children)
            {
                descendants.AddRange(child.GetDescendants());
            }
        }
        return descendants.ToArray();
    }

   public Bounds GetNodeBounds()
    {
        return _bounds;
    }
    public bool QuadContainsPoint(Vector3 point, out Quad q)
    {
        if (_bounds.Contains(point))
        {
            if(children == null || children.Length == 0)
            {
                q = this;
                return true;
            }
            foreach(var child in children)
            {
                if(child.QuadContainsPoint(point, out q))
                {
                    return true;
                }
            }
            q = null;
            return false;
        }
        else
        {
            q = null;
            return false;
        }

    }

}
