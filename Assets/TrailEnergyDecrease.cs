using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBaske.Sensors.Grid;
using System;

public class TrailEnergyDecrease : MonoBehaviour
{
    public float decreaeRate;
    // Update is called once per frame
    public GridBuffer m_SensorBuffer;
    public int gridsize;
    private void Start()
    {

    }
    int normx, normy;
    public float normFactor;
    void FixedUpdate()
    {
        m_SensorBuffer = transform.parent.GetComponent<RecommendAgent>().trailGrid;
        m_SensorBuffer.ClearChannel(0);
        foreach (Transform child in transform)
        {
            normx = Convert.ToInt32((child.localPosition.x + 50) / gridsize);
            normy = Convert.ToInt32((child.localPosition.z + 50) / gridsize);
            if (normx < 100 / gridsize && normy < 100 / gridsize && normx >= 00 / gridsize && normy >= 00 / gridsize)
                m_SensorBuffer.Write(0, normx, normy, m_SensorBuffer.Read(0, normx, normy) + child.GetComponent<TrailPoint>().energy / normFactor);
            child.GetComponent<TrailPoint>().energy -= decreaeRate;
            if (child.GetComponent<TrailPoint>().energy <= 0) Destroy(child.gameObject);
            else
                child.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, child.GetComponent<TrailPoint>().energy);
        }
    }
}
