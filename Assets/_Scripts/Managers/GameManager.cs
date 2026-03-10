using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public ResourceStarter[] resources;


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
    Food,
    Water,
    Material,
Genetics,
Population,
Entertainment,
AvailableHousing
}
