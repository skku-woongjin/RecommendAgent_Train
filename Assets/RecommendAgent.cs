using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RecommendAgent : Agent
{
    public Transform candidates;
    public int curdest = -1;
    public float enrgyDecrease;
    public float energy = 0;
    ObservationCollector obsCollector;
    public Transform owner;
    public Transform trails;

    public override void Initialize()
    {
        curdest = -1;
        obsCollector = GetComponent<ObservationCollector>();
    }

    public override void OnEpisodeBegin()
    {
        obsCollector.setRandomPosition();
        if (curdest > -1)
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
        curdest = -1;
        foreach (Transform child in trails)
        {
            Destroy(child.gameObject);
        }
        owner.position = new Vector3(0, 0, 0);
        owner.GetComponent<OwnerController>().goTo(new Vector3(0, 0, 0));
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
            curdest = action;
            owner.GetComponent<OwnerController>().goTo(candidates.GetChild(action).position);
            candidates.GetChild(action).GetComponent<FlagColor>().red();
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
        else discreteActionsOut[0] = -1;
    }

    void FixedUpdate()
    {
        if (curdest >= 0 && Vector3.SqrMagnitude(candidates.GetChild(curdest).position - owner.position) < 20)
        {
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
            energy = 1;
            curdest = -1;
        }

        if (curdest < 0)
            RequestDecision();
        RequestAction();
        energy = Mathf.Clamp01(energy - enrgyDecrease);
    }
}
