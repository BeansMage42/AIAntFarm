using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public bool Enabled = true;
    [SerializeField] private List<Transform> neighbours = new();

    [SerializeField] private float radius = 5f;
    [SerializeField, Range(-1f, 1f)] private float fovThreshold = 0f;
    
    [SerializeField] private float separationWeight = 1f;
    [SerializeField] private float cohesionWeight = 1.0f;
    [SerializeField] private float alignmentWeight = 1.0f;
    [SerializeField] private float lemmingWeight = 1.0f;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 5f;

    private Transform leader;
    
    private Vector3 movDir;
    private int boidsLayerMask;

    private void Awake()
    {
        boidsLayerMask = LayerMask.GetMask("Boids");
        movDir = transform.forward;
        
    }
    public void InitializeBoid(float SW,float CW,float AW, float LW, float MS, float RS)
    {
         separationWeight = SW;
         cohesionWeight = CW;
        alignmentWeight = AW;
        lemmingWeight = LW;

         moveSpeed = MS;
         rotationSpeed = RS;
    }

    public void SetLeader(Transform leader)
    {
        this.leader = leader;
    }
    
    public void CalculateMovement()
    {
        neighbours = GetNeighbours();

        Vector3 separation = Separation() * separationWeight;
        Vector3 cohesion = Cohesion() * cohesionWeight;
        Vector3 alignment = Alignment() * alignmentWeight;
        Vector3 lemming = Lemming() * lemmingWeight;

        Vector3 steering = separation + cohesion + alignment + lemming;
        steering.y = 0;

        if (steering.sqrMagnitude > 0.0001f)
        {
            movDir = Vector3.Lerp(movDir, steering.normalized, 0.2f).normalized;
        }
        else
        {
            movDir = transform.forward;
        }
    }

    public void UpdateMovement()
    {
        if (movDir.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(movDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        transform.position += transform.forward * (moveSpeed * Time.deltaTime);
    }

    private List<Transform> GetNeighbours()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, boidsLayerMask);
        List<Transform> foundBoids = new(hits.Length);

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform other = hits[i].transform;

            if (other == transform)
                continue;

            Vector3 toOther = (other.position - origin).normalized;
            float dot = Vector3.Dot(forward, toOther);

            if (dot >= fovThreshold)
                foundBoids.Add(other);
        }

        return foundBoids;
    }

    private Vector3 Separation()
    {
        if (neighbours.Count == 0)
            return Vector3.zero;

        Vector3 force = Vector3.zero;

        foreach (Transform neighbour in neighbours)
        {
            Vector3 away = transform.position - neighbour.position;
            float sqrDist = away.sqrMagnitude;

            if (sqrDist > 0.0001f)
                force += away.normalized / sqrDist;
        }

        return force.normalized;
    }

    private Vector3 Cohesion()
    {
        if (neighbours.Count == 0)
            return Vector3.zero;

        Vector3 center = Vector3.zero;

        foreach (Transform neighbour in neighbours)
            center += neighbour.position;

        center /= neighbours.Count;

        return (center - transform.position).normalized;
    }

    private Vector3 Alignment()
    {
        if (neighbours.Count == 0)
            return transform.forward;

        Vector3 avgHeading = Vector3.zero;

        foreach (Transform neighbour in neighbours)
            avgHeading += neighbour.forward;

        avgHeading /= neighbours.Count;

        return avgHeading.normalized;
    }

    private Vector3 Lemming()
    {
        Vector3 dir = leader.position - transform.position;
        return dir.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * radius);
    }
}