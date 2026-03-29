using JetBrains.Annotations;
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

    Resource resoure;

    List<Vector3> points = new List<Vector3>();

    private Queue<Vector3> pointsQueue = new Queue<Vector3>();
    private Queue<CollectorAnt> linedUpAnts = new Queue<CollectorAnt>();

    public bool LineUpTime;


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
        LineUp();
    }
    public void InitializeGroup(Transform Leader)
    {
        linedUpAnts.Clear();
        fromHomeToResource = new NavMeshPath();
        fromResourceToHome = new NavMeshPath();
        home = GameManager.instance.Home.transform;
        agents.AddRange(GetComponentsInChildren<Agent>());
        leader = Leader;
        int num = 1;
        foreach (Agent i in agents)
        {
            i.InitializeBoid(separationWeight,cohesionWeight,alignmentWeight,lemmingWeight,moveSpeed,rotationSpeed);
            i.SetLeader(leader);
            i.gameObject.name = "ant " + num;
            num++;
            linedUpAnts.Enqueue(i.gameObject.GetComponent<CollectorAnt>());
        }
        GroupInitialized = true;
        resoure = leader.GetComponent<AntBase>().trackResource;
        targetResource = leader.GetComponent<AntBase>().trackResource.transform;
        resoure.OnDepleteResource += ResourceDepleted;
        CreatePaths();
    }
    private void ResourceDepleted(Resource source)
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (!GroupInitialized) return;

        if(Input.GetKeyDown(KeyCode.P))StartLineUp();    

        if(leader == null)
        {
            leader = agents.First().transform;
        }
        if (!LineUpTime)
        {
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
        else
        {
            CollectorAnt antTemp = linedUpAnts.Peek();
            Debug.DrawLine(antTemp.transform.position, antTemp.transform.position + Vector3.up *10, Color.yellow,0.5f);
            if (antTemp.ReachedTarget())
            {
                if (!antTemp.IsCollecting)
                {
                    antTemp.ExtractResource();
                }
            }
        }
    }

    public void StartLineUp()
    {
        if (LineUpTime) return;
        Debug.Log("start line");
        Debug.Log($" number of ants = {linedUpAnts.Count} number of points {pointsQueue.Count} ");
        for (int i = 0; i < linedUpAnts.Count; i++) 
        {
            
            Vector3 pointTemp = pointsQueue.Dequeue();
            CollectorAnt antTemp = linedUpAnts.Dequeue();
           
            antTemp.ResourceFound(resoure, fromHomeToResource, fromResourceToHome);
            antTemp.SwitchToNavmeshControl();
            antTemp.MoveTo(pointTemp);
            pointsQueue.Enqueue(pointTemp);
            linedUpAnts.Enqueue(antTemp);
            
        }
        linedUpAnts.Peek().extractedResource += FirstInLineCollectedResource;
        LineUpTime = true;
    }
    private void FirstInLineCollectedResource()
    {
        Debug.Log("first in line collected");
        CollectorAnt antTemp = linedUpAnts.Dequeue();
        antTemp.extractedResource -= FirstInLineCollectedResource;
        antTemp.returnedHome += AddToLine;
        for(int i = 0;i < linedUpAnts.Count; i++)
        {
            MoveUpLine();
        }
        linedUpAnts.Peek().extractedResource += FirstInLineCollectedResource;
    }
    public void MoveUpLine()
    {
        
        Vector3 pointTemp = pointsQueue.Dequeue();
        CollectorAnt antTemp = linedUpAnts.Dequeue();
        Debug.Log( antTemp.name + " move up line");
        antTemp.MoveTo(pointTemp);
        pointsQueue.Enqueue(pointTemp);
        linedUpAnts.Enqueue(antTemp);
    }
    public void AddToLine(CollectorAnt ant)
    {
        Debug.Log($"add {ant.name} to line");
        ant.returnedHome -= AddToLine;
        Vector3 pointTemp = pointsQueue.Dequeue();
        ant.MoveTo(pointTemp);
        pointsQueue.Enqueue(pointTemp);
        linedUpAnts.Enqueue(ant);

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
        if (pointsQueue.Count > 0) pointsQueue.Clear();
            float space = 0f;
            float antSpacing = 2 + 0.2f;//twice radius + a bit of extra
        space += 1;
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
                if (NavMesh.SamplePosition(positionToPlace, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
                {
                    positionToPlace = hit.position;
                }
            }
            space += antSpacing;
            
            pointsQueue.Enqueue(positionToPlace);
           //GameObject newObj = Instantiate(targetResource.gameObject,positionToPlace, Quaternion.identity);
          // newObj.name = "test " + i;
        }
        
    }
}
