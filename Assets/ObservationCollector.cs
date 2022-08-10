using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation.Samples;
using TMPro;
using System;
using Unity.MLAgents.Sensors;

public class ObservationCollector : MonoBehaviour
{
    public int worldSize;
    public int cellSize;
    int gridsize;
    public int tagcount;
    public int flagCount;
    public Transform candidates;
    public GameObject flagPrefab;

    float[,,] grid;

    private void Start()
    {
        if (candidates.childCount == 0)
        {
            for (int i = 0; i < flagCount; i++)
            {
                GameObject tmp = Instantiate(flagPrefab, candidates);
                tmp.GetComponentInChildren<TMP_Text>().text = i + "";
            }
        }
        setRandomPosition();

    }

    public void setRandomPosition()
    {
        foreach (Transform child in candidates)
        {
            child.transform.localPosition = new Vector3(UnityEngine.Random.Range(-worldSize / 2, worldSize / 2), 0, UnityEngine.Random.Range(-worldSize / 2, worldSize / 2));
            // if (child.GetChild(0).GetComponent<NavMeshSourceTag>() != null)
            //     child.GetChild(0).GetComponent<NavMeshSourceTag>().enabled = true;
        }
        // GetComponent<LocalNavMeshBuilder>().UpdateNavMesh(true);
    }


    public Transform trails;

    public float[,,] getGrid()
    {
        return grid;
    }
}
