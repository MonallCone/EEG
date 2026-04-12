using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UriGellerMovement : MonoBehaviour
{
    public Transform player;

    [Header("Vision")]
    public float visionDistance = 10f;
    public float fov = 90f;

    [Header("Patrol Paths")]
    public Transform[] pathA;
    public Transform[] pathB;
    public Transform[] pathC;
    public Transform[] pathD;

    private Transform[] currentPath;
    private int currentPathIndex = 0;

    private NavMeshAgent agent;

    private enum State {Chase, Patrol}
    private State currentState = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentPath = pathA;
    }

    void Update()
    {
        if (canSeePlayer())
        {
            currentState = State.Chase;
        }
        else if (currentState == State.Chase)
        {
            // Lost player → go back to patrol
            currentState = State.Patrol;

            currentPath = getNearestPath();
            currentPathIndex = getNearestPointIndex(currentPath);
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    void Patrol()
    {
        if (currentPath == null || currentPath.Length == 0) return;

        agent.SetDestination(currentPath[currentPathIndex].position);

        // Check if agent reached destination properly
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPathIndex = (currentPathIndex + 1) % currentPath.Length;
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);
    }

    bool canSeePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > visionDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > fov / 2f)
            return false;

        Ray ray = new Ray(transform.position + Vector3.up, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, visionDistance))
        {
            if (hit.transform == player)
                return true;
        }

        return false;
    }

    Transform[] getNearestPath()
    {
        Transform[][] allPaths = new Transform[][] {pathA, pathB, pathC, pathD};

        Transform[] nearestPath = pathA;
        float shortestDistance = Mathf.Infinity;

        foreach(Transform[] path in allPaths)
        {
            foreach (Transform point in path)
            {
                float distance = Vector3.Distance(transform.position, point.position);

                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPath = path;
                }
            }
        }

        return nearestPath;
    }

    int getNearestPointIndex(Transform[] path)
    {
        int nearestIndex = 0;
        float shortestDistance = Mathf.Infinity;

        for (int i = 0; i < path.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, path[i].position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }
}
