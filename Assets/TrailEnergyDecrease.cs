using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEnergyDecrease : MonoBehaviour
{
    public float decreaeRate;
    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<TrailPoint>().energy -= decreaeRate;
            if (child.GetComponent<TrailPoint>().energy <= 0) Destroy(child.gameObject);
        }
    }
}
