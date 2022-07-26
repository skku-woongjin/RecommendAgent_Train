using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSensor : MonoBehaviour
{
    public IdleAgent agent;
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

            if (other.CompareTag("target"))
            {
                agent.obstacle = other.transform;
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
