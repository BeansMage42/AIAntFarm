using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class Quad
{
    //Vector3 _center;
    [DoNotSerialize]
    public Bounds _bounds;
    public int _generation;
    int _limit;
    Quad _parent;
    Quad[] children = null;
    List<QuadTreeObject> containedObjects = new();
    public Dictionary<ResourceType, (Resource,float)> _scents = new Dictionary<ResourceType,(Resource, float)>();
    public float Strength;
    public Quad(int _g, Quad _par, Bounds _b, int _l)
    {
        _parent = _par;
        _generation = _g;
        _bounds = _b;
        _limit = _l;
    }
    public void AddObject(QuadTreeObject go)
    {
        DivideAndAdd(go);
       
    }

    private void DivideAndAdd(QuadTreeObject go)
    {
        if (_generation >= _limit) 
        {
            if (go is Resource)
            {
               
             // _scents.Add((go as Resource).resourceType, CalculateScentStrength((Resource)go));
             float strength = CalculateScentStrength((Resource)go);
                if(_scents.ContainsKey((go as Resource).resourceType))
                {
                    if (_scents[(go as Resource).resourceType].Item2 < strength)
                    {
                        _scents[(go as Resource).resourceType] = ((Resource)go,strength);
                    }
                }
                else
                {
                    _scents[(go as Resource).resourceType] = ((Resource)go, strength);
                }
                    
                
            }
            return; 
        }
        if (children == null)
        {


            children = new Quad[4];
            float length = _bounds.size.x / 4;
            float height = _bounds.size.z / 4;
            Vector3 childSize = new Vector3(length, 5, height) * 2;
            children[0] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(-length, 0, -height), childSize), _limit);
            children[1] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(length, 0, -height), childSize), _limit);
            children[2] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(length, 0, height), childSize), _limit);
            children[3] = new Quad(_generation + 1, this, new Bounds(_bounds.center + new Vector3(-length, 0, height), childSize), _limit);
        }
        containedObjects.Add(go);

        foreach (var child in children)
        {
            if(child.QuadIntersectsBounds(go.bounds,out Quad[] intersecting))
            {
                child.DivideAndAdd(go);
            }
        }
       

    }

    private float CalculateScentStrength(Resource go)
    {
        Vector3 objPos = go.bounds.center;
        float strength = 1 - (Vector3.Distance(this._bounds.center,objPos)/go.bounds.size.z);
        Strength = strength;
        //Debug.Log( "strength of resource " + go.name + " is " + strength + " at distance " + Vector3.Distance(this._bounds.center, objPos));
        return strength;
    }

    

    public void Subdivide()
    {
        if(_generation >=  _limit) return;
        children = new Quad[4];
        float length = _bounds.size.x/4;
        float height = _bounds.size.z/4;
        Vector3 childSize = new Vector3(length, 5, height)*2;
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
           // Debug.Log("contained point");
            if(children == null || children.Length == 0)
            {
                q = this;
                return true;
            }
           // Debug.Log("checking children");
            foreach(var child in children)
            {
                if(child.QuadContainsPoint(point, out q))
                {
                    
                    return true;
                }
            }
            q = this;
            return true;
        }
        else
        {
          //  Debug.Log("child did not contain");
            q = null;
            return false;
        }

    }
    public bool QuadIntersectsBounds(Bounds bounds, out Quad[] intersectingQuads)
    {
        List<Quad> list = new List<Quad>();
        if (_bounds.Intersects(bounds))
        {
           // Debug.Log("intersected");
            if (children == null || children.Length == 0)
            {
                list.Add(this);
                intersectingQuads = list.ToArray();
                return true;
            }
           // Debug.Log("checking children");
            foreach (var child in children)
            {
                if (child.QuadIntersectsBounds(bounds, out Quad[] test))
                {
                    list.AddRange(test);
                }
            }
            intersectingQuads = list.ToArray();
            return true;
        }
        else
        {
           // Debug.Log("child did not contain");
            intersectingQuads = null;
            return false;
        }
    }


}
