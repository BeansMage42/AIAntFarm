using UnityEngine;

public abstract class QuadTreeObject:MonoBehaviour
{
    public Bounds bounds;

    private void OnDestroy()
    {
        
    }
    public virtual void OnEnable()
    {
        bounds = GetComponent<Collider>().bounds;
       // QuadTreeManager.Instance.AddObjectToTree(this);
    }
    private void OnDisable()
    {
        //QuadTreeManager.Instance.RemoveObjectFromTree(this);
    }

    public virtual void OnPlace()
    {
        QuadTreeManager.Instance.AddObjectToTree(this);
    }
}
