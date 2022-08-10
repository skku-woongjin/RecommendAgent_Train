using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using TMPro;

public class RecommendAgent : Agent
{
    public Transform candidates;
    public int curdest = -1;
    public float enrgyDecrease;
    public float energy = 0;
    ObservationCollector obsCollector;
    public Transform owner;
    public Transform trails;

    int flagcount;
    public int destQSize;
    int destQfilled;
    Queue<int> destQ;
    int[] flagVisited;
    bool going = false;

    public int epLength;
    int curep;

    public override void Initialize()
    {
        obsCollector = GetComponent<ObservationCollector>();
        flagcount = obsCollector.flagCount;
        destQ = new Queue<int>();
        flagVisited = new int[obsCollector.flagCount];
        curdest = -1;
    }

    public override void OnEpisodeBegin()
    {
        energy = 0;
        rew = 0;
        obsCollector.setRandomPosition();
        if (curdest > -1)
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
        curdest = -1;
        foreach (Transform child in trails)
        {
            Destroy(child.gameObject);
        }
        owner.localPosition = new Vector3(0, 0, 0);
        owner.GetComponent<OwnerController>().goTo(transform.position);
        owner.GetComponent<OwnerController>().resetQ();

        destQ.Clear();
        Array.Clear(flagVisited, 0, flagVisited.Length);
        destQfilled = 0;
        going = false;
        updateFlagUI();
        curep = 0;
    }

    void updateFlagUI()
    {
        for (int i = 0; i < candidates.childCount; i++)
        {
            candidates.GetChild(i).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = flagVisited[i] + "";
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(energy);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var action = actionBuffers.DiscreteActions[0];
        if (action != -1 && action < candidates.childCount)
        {
            if (going) return;
            going = true;
            curdest = action;
            owner.GetComponent<OwnerController>().goTo(candidates.GetChild(action).position);
            candidates.GetChild(action).GetComponent<FlagColor>().red();
            float g = destQSize / flagcount;

            if (flagVisited[action] <= g)
            {
                AddReward(1 - flagVisited[action] / g);
            }
            else
            {
                AddReward((1 - flagVisited[action] / g) / 7);
            }

            if (destQfilled == destQSize)
            {
                flagVisited[destQ.Dequeue()]--;
                destQfilled--;
            }
            destQ.Enqueue(action);
            destQfilled++;
            flagVisited[action]++;
            updateFlagUI();
            curep++;
            if (curep == epLength)
                EndEpisode();
        }

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
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
    public float rew;
    void FixedUpdate()
    {
        if (curdest >= 0 && Vector3.SqrMagnitude(candidates.GetChild(curdest).position - owner.position) < 20)
        {
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
            going = false;
            energy = 1;
            curdest = -1;
        }

        if (curdest < 0)
            RequestDecision();
        RequestAction();
        energy = Mathf.Clamp01(energy - enrgyDecrease);
    }
}
