using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationCollector : MonoBehaviour
{
    public int gridsize;
    public int tagcount;
    float[][][] grid;

    private void Start()
    {
        grid = new float[100 / gridsize, 100 / gridsize, tagcount];
    }
    public float[][][] getObs()
    {

    }
}
