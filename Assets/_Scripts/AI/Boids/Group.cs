using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class Group : MonoBehaviour
{
    private Transform leader;
    List<Agent> agents = new();
    [SerializeField] private float radius = 5f;
    [SerializeField, Range(-1f, 1f)] private float fovThreshold = 0f;

    [SerializeField] private float separationWeight = 1f;
    [SerializeField] private float cohesionWeight = 1.0f;
    [SerializeField] private float alignmentWeight = 1.0f;
    [SerializeField] private float lemmingWeight = 1.0f;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 5f;

    bool GroupInitialized;

    NavMeshPath fromHomeToResource ;
    NavMeshPath fromResourceToHome ;
    public Transform home;
    public Transform targetResource;

    List<Vector3> points = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
      //  agents = GetComponentsInChildren<Agent>();
        
       
    }

    public void CreatePaths()
    {
        Vector3 fromTO = targetResource.position - home.position;
        Vector2 normal = new Vector2(-fromTO.z, fromTO.x).normalized ;
        Vector3 offset = new Vector3(normal.x, 0, normal.y);
        if (NavMesh.CalculatePath(targetResource.position + offset, home.position + offset, NavMesh.AllAreas, fromResourceToHome)) Debug.Log("found path to resource");
        if (NavMesh.CalculatePath(home.position - offset, targetResource.position - offset, NavMesh.AllAreas, fromHomeToResource)) Debug.Log("found path to home");

        Debug.Log("toresource length " + fromHomeToResource.corners.Length);
        Debug.Log("tohome length " + fromResourceToHome.corners.Length);

        foreach (Vector3 point in fromHomeToResource.corners)
        {
            Debug.DrawLine(point,point + Vector3.up*5, Color.blue,2f);
        }
        foreach (Vector3 point in fromResourceToHome.corners)
        {
            Debug.DrawLine(point, point + Vector3.up * 5, Color.red, 2f);
        }
    }
    public void InitializeGroup(Transform Leader)
    {
        fromHomeToResource = new NavMeshPath();
        fromResourceToHome = new NavMeshPath();
        home = GameManager.instance.Home.transform;
        agents.AddRange(GetComponentsInChildren<Agent>());
        leader = Leader;
        foreach (Agent i in agents)
        {
            i.InitializeBoid(separationWeight,cohesionWeight,alignmentWeight,lemmingWeight,moveSpeed,rotationSpeed);
            i.SetLeader(leader);
        }
        GroupInitialized = true;
        targetResource = leader.GetComponent<AntBase>().trackResource.transform;
        CreatePaths();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GroupInitialized) return;

        if(Input.GetKeyDown(KeyCode.P))LineUp();    

        if(leader == null)
        {
            leader = agents.First().transform;
        }
        foreach (Agent i in agents)
        {
            if (i.Enabled)
                i.CalculateMovement();
        }
        foreach (Agent i in agents)
        {
            if (i.Enabled)
                i.UpdateMovement();
        }
    }

    public void ChangeGroupLeader(Transform newLeader)
    {
        leader = newLeader;
        foreach(Agent i in agents)
        {
            i.SetLeader(leader);
        }
    }

    public void LineUp()
    {
        
            float space = 0f;
            float antSpacing = 2 + 0.1f;//twice radius + a bit of extra

        for(int i = 0; i < agents.Count; i++)
        {
            int lineIndex = fromHomeToResource.corners.Length - 1;
            Vector3 lineStart = fromHomeToResource.corners[lineIndex];
            Vector3 lineEnd = fromHomeToResource.corners[lineIndex - 1];
            Vector3 LineSegment = lineEnd - lineStart;
            float spaceAlongSegment = LineSegment.magnitude;
            Vector3 positionToPlace = Vector3.zero;
            float spaceRemaining = space;
            while (spaceRemaining > spaceAlongSegment)
            {
                spaceRemaining -= spaceAlongSegment;
                 lineIndex --;
                   if(lineIndex <= 0) break;
                 lineStart = fromHomeToResource.corners[lineIndex];
                 lineEnd = fromHomeToResource.corners[lineIndex - 1];
                 LineSegment = lineEnd - lineStart;
                spaceAlongSegment = LineSegment.magnitude;
            }
            if(lineIndex != 0)
            {
                positionToPlace = Vector3.Lerp(lineStart, lineEnd, spaceRemaining/spaceAlongSegment);
            }
            else
            {
                positionToPlace = fromHomeToResource.corners[0] + LineSegment.normalized * space;
            }
            space += antSpacing;
            points.Add(positionToPlace);
           GameObject newObj = Instantiate(targetResource.gameObject,positionToPlace, Quaternion.identity);
           newObj.name = "test " + i;
        }
        
    }
}
