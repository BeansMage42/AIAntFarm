using UnityEngine;

public sealed class World
{
    private static readonly World instance = new World();
    
    private static WorldStates world = null;
    
    private static AntSpawner antspawner;

    static World()
    {
        world = new WorldStates();

        if (GameObject.FindGameObjectWithTag("AntSpawner").TryGetComponent<AntSpawner>(out antspawner))
        {
        }
        else
        {
            Debug.Log("Oops there is no antspawner");
        }
    }
    World()
    {
        
    }

    public void SpawnSeeker()
    {
        antspawner.SpawnSeeker(ResourceType.Food);
    }

    public static World Instance
    {
        get {return instance;}  
    }

    public WorldStates GetWorld()
    {
        return world;
    }
    
}
