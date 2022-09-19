using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System;
using TMPro;
using MBaske.Sensors.Grid;
using Unity.MLAgents.Policies;
using System.Linq;

public class RecommendAgent : Agent
{
    public bool autoHeuristic;
    public bool debugReward;
    public bool warp;
    public Transform candidates;
    public int curdest = -1;

    public GameObject flagPrefab;
    public Transform owner;
    public Transform trails;

    public int flagCount;
    public int destQSize;
    int destQfilled;
    Queue<int> destQ;
    int[] flagVisited;
    bool going = false;

    public int epLength;
    int curep;

    public GridBuffer flagGrid;
    public GridBuffer trailGrid;

    public GridSensorComponent flagGridComp;
    public GridSensorComponent trailGridComp;
    public int cellSize;
    public int worldSize;

    public Vector3[] flagpos;

    void setRandomPosition()
    {
        flagGrid.ClearChannel(0);
        for (int i = 0; i < flagCount; i++)
        {
            flagpos[i] = new Vector3(UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2), 0, UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2));
        }

        flagpos = flagpos.OrderBy(v => -v.z).ThenBy(v => v.x).ToArray<Vector3>();

        int j = 0;
        foreach (Transform child in candidates)
        {
            child.transform.localPosition = flagpos[j];
            GetComponent<RecommendAgent>().flagGrid.Write(0, Convert.ToInt32((child.localPosition.x + 50) / cellSize), Convert.ToInt32((child.localPosition.z + 50) / cellSize), 1);
            j++;
        }
    }

    public override void Initialize()
    {
        if (candidates.childCount == 0)
        {
            for (int i = 0; i < flagCount; i++)
            {
                GameObject tmp = Instantiate(flagPrefab, candidates);
                tmp.GetComponentInChildren<TMP_Text>().text = i + "";
            }
        }

        flagGrid = new ColorGridBuffer(1, worldSize / cellSize, worldSize / cellSize);
        flagGridComp.GridBuffer = flagGrid;
        trailGrid = new ColorGridBuffer(1, worldSize / cellSize, worldSize / cellSize);
        trailGridComp.GridBuffer = trailGrid;
        destQ = new Queue<int>();
        flagVisited = new int[flagCount];
        flagpos = new Vector3[flagCount];
        curdest = -1;

    }

    public override void OnEpisodeBegin()
    {
        rew = 0;
        setRandomPosition();
        if (curdest > -1)
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
        curdest = -1;
        foreach (Transform child in trails)
        {
            Destroy(child.gameObject);
        }
        owner.localPosition = new Vector3(0, 0, 0);
        destQ.Clear();
        Array.Clear(flagVisited, 0, flagVisited.Length);
        destQfilled = 0;
        going = false;
        curep = 0;
        owner.GetComponent<OwnerController>().goTo(-1);
        if (warp)
            owner.GetComponent<OwnerController>().randomWarp(UnityEngine.Random.Range(15, destQSize - 1));
        RequestDecision();

    }

    public void updateFlags(int dest)
    {
        if (dest >= 0)
        {
            if (destQfilled == destQSize)
            {
                flagVisited[destQ.Dequeue()]--;
                destQfilled--;
            }

            destQ.Enqueue(dest);
            destQfilled++;
            flagVisited[dest]++;
        }

        for (int i = 0; i < candidates.childCount; i++)
        {
            candidates.GetChild(i).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = flagVisited[i] + "";
        }
    }

    public bool showResult;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var action = actionBuffers.DiscreteActions[0];
        if (action != -1 && action < candidates.childCount)
        {
            going = true;

            curdest = action;
            if (!warp)
                owner.GetComponent<OwnerController>().goTo(action);
            candidates.GetChild(action).GetComponent<FlagColor>().red();

            float g = destQSize / flagCount;

            rew += 1 - flagVisited[action] / g;
            AddReward(1 - flagVisited[action] / g);

            if (debugReward)
                Debug.Log("id: " + action + "\n #visited: " + flagVisited[action] + " reward: " + (1 - flagVisited[action] / g));

            if (warp)
            {
                if (showResult)
                    Invoke("EndEpisode", 2f);
                else
                    EndEpisode();
                return;
            }
            curep++;
            if (curep == epLength)
                EndEpisode();
        }
        else
        {
            Debug.Log(action);
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (!autoHeuristic)
        {

            if (Input.GetKey(KeyCode.Alpha0)) { discreteActionsOut[0] = 0; }
            else if (Input.GetKey(KeyCode.Alpha1)) discreteActionsOut[0] = 1;
            else if (Input.GetKey(KeyCode.Alpha2)) discreteActionsOut[0] = 2;
            else if (Input.GetKey(KeyCode.Alpha3)) discreteActionsOut[0] = 3;
            else if (Input.GetKey(KeyCode.Alpha4)) discreteActionsOut[0] = 4;
            else if (Input.GetKey(KeyCode.Alpha5)) discreteActionsOut[0] = 5;
            else if (Input.GetKey(KeyCode.Alpha6)) discreteActionsOut[0] = 6;
            else if (Input.GetKey(KeyCode.Alpha7)) discreteActionsOut[0] = 7;
            else if (Input.GetKey(KeyCode.Alpha8)) discreteActionsOut[0] = 8;
            else if (Input.GetKey(KeyCode.Alpha9)) discreteActionsOut[0] = 9;
            else if (Input.GetKey(KeyCode.Q)) discreteActionsOut[0] = 10;
            else if (Input.GetKey(KeyCode.W)) discreteActionsOut[0] = 11;
            else if (Input.GetKey(KeyCode.E)) discreteActionsOut[0] = 12;
            else if (Input.GetKey(KeyCode.R)) discreteActionsOut[0] = 13;
            else if (Input.GetKey(KeyCode.T)) discreteActionsOut[0] = 14;
            else if (Input.GetKey(KeyCode.Y)) discreteActionsOut[0] = 15;
            else if (Input.GetKey(KeyCode.U)) discreteActionsOut[0] = 16;
            else if (Input.GetKey(KeyCode.I)) discreteActionsOut[0] = 17;
            else if (Input.GetKey(KeyCode.O)) discreteActionsOut[0] = 18;
            else if (Input.GetKey(KeyCode.P)) discreteActionsOut[0] = 19;
            else if (Input.GetKey(KeyCode.A)) discreteActionsOut[0] = 20;
            else discreteActionsOut[0] = -1;
        }
        else
        {
            int min = 1000;
            int action = -1;
            for (int i = 0; i < flagCount; i++)
            {
                if (flagVisited[i] < min)
                {
                    action = i;
                    min = flagVisited[i];
                }
            }
            discreteActionsOut[0] = action;
        }
    }
    public float rew;
    void FixedUpdate()
    {
        if (!warp && going && curdest >= 0 && Vector3.SqrMagnitude(candidates.GetChild(curdest).position - owner.position) < 10)
        {
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
            going = false;
            curdest = -1;
            RequestDecision();
        }
    }
}
