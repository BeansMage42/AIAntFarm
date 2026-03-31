using UnityEngine;

public sealed class World
{
    private static readonly World instance = new World();
    
    private static WorldStates world = null;

    static World()
    {
        world = new WorldStates();
    }
    World()
    {
        
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
