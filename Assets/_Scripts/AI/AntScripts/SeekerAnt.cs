using UnityEngine;

public class SeekerAnt:AntBase
{
    public ResourceType _seekingResourceType;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Resource resource))
        {

        }
    }

}
