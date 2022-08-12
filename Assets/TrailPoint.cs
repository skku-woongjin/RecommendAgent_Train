using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBaske.Sensors.Grid;

public class TrailPoint : MBaske.Sensors.Grid.DetectableGameObject
{
    public float energy;
    public int gridW;
    public int gridH;

    private GridBuffer m_SensorBuffer;

    float getEnergy()
    {
        return energy;
    }
    public override void AddObservables()
    {
        Observables.Add("Energy", getEnergy);
    }

    void Awake()
    {

    }

}
