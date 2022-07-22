using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InterestingSensor : MonoBehaviour

{

    public IdleAgent agent;
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "target")
        {
            agent.interestingObj = other.transform;
            agent.interest();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == agent.interestingObj)
        {
            agent.endInterest();
        }
    }
}