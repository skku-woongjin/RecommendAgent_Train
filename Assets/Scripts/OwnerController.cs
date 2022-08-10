using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OwnerController : MonoBehaviour
{
    public Animator animator;
    NavMeshAgent nav;
    public float traceSpacing;


    void OnEnable()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.updatePosition = true;
        nav.updateRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    public void goTo(Vector3 pos)
    {
        nav.SetDestination(pos);
    }

    //NOTE Trace 관련 
    public GameObject tracePrefab;
    public Transform traces;
    public Queue<Vector3> waypoints;
    public int queueSize;
    int queueFilled = 0;
    Vector3 lastpos;
    public RecommendAgent agent;

    private void Start()
    {
        waypoints = new Queue<Vector3>();
    }

    private void FixedUpdate()
    {
        if (lastpos == null || Vector3.SqrMagnitude(lastpos - transform.position) > traceSpacing)
        {
            if (agent.energy > 0)
            {
                GameObject trailPoint = Instantiate(tracePrefab, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity, traces);
            }
            if (queueFilled == queueSize)
            {
                waypoints.Dequeue();
                queueFilled -= 1;
            }
            waypoints.Enqueue(transform.position);
            queueFilled++;
            lastpos = transform.position;
            // if (queueFilled > 0)
            //     agent.AddReward(Vector3.SqrMagnitude(waypoints.Peek() - transform.position) / 10000);
            // agent.AddReward(agent.energy / 10);

        }
    }

    public void resetQ()
    {
        waypoints.Clear();
        queueFilled = 0;
    }

}
