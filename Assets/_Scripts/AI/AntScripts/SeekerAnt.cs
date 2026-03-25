using Unity.Behavior;
using UnityEngine;

public class SeekerAnt:AntBase
{
    public ResourceType _seekingResourceType;
    public QuadTreeManager _quadTreeManager;
    public Collider _antBounds;
    public Resource trackResource;
    public EventChannel<Resource,float> _foundResourceChannel;
    private void Start()
    {
        _quadTreeManager = QuadTreeManager.Instance;
        _antBounds = GetComponent<Collider>();
       // _antTree.GetVariable("")
    }
    private void FixedUpdate()
    {
        
    }

    public (Resource,float) QueryTreeForScent(int type)
    {
        _seekingResourceType = (ResourceType)type;
        if (_quadTreeManager.TreeContainsBounds(_antBounds.bounds, out Quad[] searchForIntersections))
        {
            Resource bestResource = null;
            float strongest = -float.MaxValue;
            foreach (var quad in searchForIntersections)
            {
                foreach (var scent in quad._scents.Keys)
                {
                    if (scent == _seekingResourceType)
                    {
                        if (quad._scents[scent].Item2 > strongest)
                        {
                            bestResource = quad._scents[scent].Item1;
                            strongest = quad._scents[scent].Item2;
                        }
                    }
                }
            }
            if (bestResource != null)
            {
                trackResource = bestResource;
                return (trackResource,strongest);
            }
            else
            {
                trackResource = null;
                return (trackResource,0);
            }
        }
        return (null, 0);
    }

}
