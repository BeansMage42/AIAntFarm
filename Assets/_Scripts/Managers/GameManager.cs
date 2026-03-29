using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public ResourceStarter[] resources;

    public GameObject Home;
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        else
        {
            instance = this;
        }
    }

    
}

[Serializable]
public struct ResourceStarter
{
   public ResourceType type;
   public float startingAmount;
}
public enum ResourceType
{
    Food = 1,
    Water = 2,
    Material = 3,
Genetics = 4,
Population = 5,
Entertainment = 6,
AvailableHousing = 7
}
