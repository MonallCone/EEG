using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UriGellerMovement : MonoBehaviour
{
    public Transform player;

    [Header("Wandering")]
    public float wanderRadius = 10f;
    public float wanderDelay = 3f;

    [Header("Vision")]
    public float visionDistance = 10f;
    public float fov = 90f;

    private NavMeshAgent agent;
    private float wanderTimer;

    private enum State {Wander, Chase}
    private State currentState = State.Wander;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderDelay;
    }

    void Update()
    {
        if (canSeePlayer())
        {
            currentState = State.Chase;
        }
        else if(currentState == State.Chase)
        {
            currentState = State.Wander;
        }

                // Run behaviour
        switch (currentState)
        {
            case State.Wander:
                Wander();
                break;

            case State.Chase:
                Chase();
                break;
        }
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if(wanderTimer >= wanderDelay)
        {
            Vector3 newPos = getRandomPoint(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            wanderTimer = 0;
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

    Vector3 getRandomPoint(Vector3 position, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }
}
