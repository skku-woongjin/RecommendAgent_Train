using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestingSensor : MonoBehaviour
{
    public IdleAgent agent;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "target")
        {
            agent.interest();
            agent.interestingObj = other.transform;
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
