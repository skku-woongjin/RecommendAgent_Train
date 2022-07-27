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
    private void OnTriggerStay(Collider other)
    {

        if (!other.Equals(agent.interestingObj))
        {
            if (other.CompareTag("Player") && agent.state == IdleAgent.States.rand)
            {
                agent.ObstAgent(other.transform);
            }
            if (other.CompareTag("wall") || other.CompareTag("Obstacle"))
            {
                if (!GameManager.Instance.ingroup)
                    agent.ObstAgent(other.transform);
            }

            if (other.CompareTag("target"))
            {
                if (agent.state == IdleAgent.States.inte)
                    agent.obstacle = other.transform;
                else
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
