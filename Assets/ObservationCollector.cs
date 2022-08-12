using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation.Samples;
using TMPro;
using System;
using MBaske.Sensors.Grid;

public class ObservationCollector : MonoBehaviour
{
    public int worldSize;
    public int gridsize;
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
        GetComponent<RecommendAgent>().flagGrid.ClearChannel(0);
        foreach (Transform child in candidates)
        {

            child.transform.localPosition = new Vector3(UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2), 0, UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2));
            GetComponent<RecommendAgent>().flagGrid.Write(0, Convert.ToInt32((child.localPosition.x + 50) / gridsize), Convert.ToInt32((child.localPosition.z + 50) / gridsize), (child.GetSiblingIndex() + 1.0f) / (candidates.childCount + 1));
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
