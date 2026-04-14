using _Scripts.AI.Utility.QuadTree;
using UnityEngine;

public abstract class QuadTreeObject:MonoBehaviour
{
    public Bounds bounds;
    public Poolable poolable;
    private void OnDestroy()
    {
        
    }
    public virtual void OnEnable()
    {
        poolable = GetComponent<Poolable>();
        bounds = GetComponent<Collider>().bounds;
       // QuadTreeManager.Instance.AddObjectToTree(this);
    }
    private void OnDisable()
    {
        //QuadTreeManager.Instance.RemoveObjectFromTree(this);
    }

    public virtual void OnPlace()
    {
        bounds = GetComponent<Collider>().bounds;
        QuadTreeManager.Instance.AddObjectToTree(this);
    }

    public virtual void DisableObject()
    {
        QuadTreeManager.Instance.RemoveObjectFromTree(this);
        poolable.Pool.ReturnToPool(gameObject);
    }
}
