using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSensor : MonoBehaviour
{
    public IdleAgent agent;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        transform.position = agent.transform.position + Vector3.forward * 0.3f;
        transform.rotation = agent.transform.rotation;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.Equals(agent.interestingObj))
        {
            if (other.CompareTag("wall") || other.CompareTag("Obstacle") || other.CompareTag("Player"))
            {
                agent.ObstAgent(other.transform);
            }

            if (other.CompareTag("target") && (agent.interestingObj == null || other.gameObject != agent.interestingObj.gameObject))
            {
                agent.ObstAgent(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (agent.state != IdleAgent.States.outbound && agent.obstacle != null && agent.obstacle.gameObject == other.gameObject)
        {
            agent.endObst();
        }
    }

}
