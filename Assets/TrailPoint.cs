using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPoint : MBaske.Sensors.Grid.DetectableGameObject
{
    public float energy;

    float getEnergy()
    {
        return energy;
    }
    public override void AddObservables()
    {
        Observables.Add("Energy", getEnergy);
    }

}
