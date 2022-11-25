using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using TMPro;
using MBaske.Sensors.Grid;
using Unity.MLAgents.Policies;
using System.Linq;

public class Flag
{
    public Vector3 pos = new Vector3(0, 0, 0);
    public int visited = 0;
    public float time = 0;
}

public class RecommendAgent : Agent
{
    public bool trivariate = false;
    public bool getMean;
    public int logLen = 20;
    public bool autoHeuristic;
    public bool debugReward;
    public bool warp;
    public Transform candidates;
    public int curdest = -1;
    public GameObject flagPrefab;
    public Transform owner;
    public Transform trails;

    [HideInInspector]
    public Flag[] flags;
    Flag[] flageval;
    public int flagCount;
    public int destQSize;
    int destQfilled;
    Queue<int> destQ;

    bool going = false;

    public int epLength;
    int curep;

    public GridBuffer flagGrid;
    public GridBuffer trailGrid;

    // public MBaske.Sensors.Grid.GridSensorComponent trailGridComp;
    public int cellSize;
    public int worldSize;

    Vector2 userMean;
    Vector3 userMean3;
    Vector2[] userLog;
    Vector3[] userLog3;

    void setRandomPosition()
    {
        // flagGrid.ClearChannel(0);
        for (int i = 0; i < flagCount; i++)
        {
            flags[i].pos = new Vector3(UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2), 0, UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2));
        }

        flags = flags.OrderBy(v => -v.pos.z).ThenBy(v => v.pos.x).ToArray<Flag>();

        int j = 0;
        foreach (Transform child in candidates)
        {
            child.transform.localPosition = flags[j].pos;
            // GetComponent<RecommendAgent>().flagGrid.Write(0, Convert.ToInt32((child.localPosition.x + 50) / cellSize), Convert.ToInt32((child.localPosition.z + 50) / cellSize), 1);
            j++;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        if (getMean)
        {
            if (trivariate)
            {
                foreach (Vector3 vec in userLog3)
                    sensor.AddObservation(vec);
            }
            else
            {
                foreach (Vector2 vec in userLog)
                    sensor.AddObservation(vec);
            }
        }
        else
        {
            if (trivariate)
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    sensor.AddObservation(new Vector3(flags[i].visited, Vector3.Distance(flags[i].pos, owner.localPosition), flags[i].time));
                }
            }
            else
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    sensor.AddObservation(new Vector2(flags[i].visited, Vector3.Distance(flags[i].pos, owner.localPosition)));
                }
            }

        }


    }



    float maxGaus = 0;

    public GameObject dotPrefab;
    public GameObject agenDotPrefab;
    public GameObject answerDotPrefab;
    public Transform graph;

    public override void Initialize()
    {
        if (candidates.childCount == 0)
        {
            flagCount = 8;
            // flagCount = (int)(Academy.Instance.EnvironmentParameters.GetWithDefault("block_offset", numOfFlags));
            for (int i = 0; i < flagCount; i++)
            {
                GameObject tmp = Instantiate(flagPrefab, candidates);
                tmp.GetComponentInChildren<TMP_Text>().text = i + "";
            }
        }

        if (graph.childCount == 0)
        {
            Instantiate(answerDotPrefab, graph);
            Instantiate(agenDotPrefab, graph);

            for (int i = 0; i < logLen; i++)
            {
                Instantiate(dotPrefab, graph);
            }
        }
        maxcount = destQSize;


        // flagGrid = new ColorGridBuffer(1, worldSize / cellSize, worldSize / cellSize);
        // flagGridComp.GridBuffer = flagGrid;
        // trailGrid = new ColorGridBuffer(1, worldSize / cellSize, worldSize / cellSize);
        // trailGridComp.GridBuffer = trailGrid;
        destQ = new Queue<int>();
        userLog = new Vector2[logLen];
        userLog3 = new Vector3[logLen];
        flags = new Flag[flagCount];
        for (int i = 0; i < flagCount; i++)
        {
            flags[i] = new Flag();
        }

        curdest = -1;
        setRandomPosition();

    }

    public override void OnEpisodeBegin()
    {
        setRandomPosition();
        rew = 0;

        if (curdest > -1)
            candidates.GetChild(curdest).GetComponent<FlagColor>().yellow();
        curdest = -1;
        foreach (Transform child in trails)
        {
            Destroy(child.gameObject);
        }
        destQ.Clear();
        foreach (Flag flag in flags)
        {
            flag.visited = 0;
        }

        destQfilled = 0;
        going = false;
        curep = 0;
        owner.GetComponent<TrailGenerator>().goTo(-1);
        //방문횟수 생성
        if (warp)
            owner.GetComponent<TrailGenerator>().randomWarp(UnityEngine.Random.Range(15, destQSize - 1));
        //유저 위치 랜덤 생성
        owner.localPosition = new Vector3(UnityEngine.Random.Range(-50, 50), 0, UnityEngine.Random.Range(-50, 50));

        //평균 방문 시간 생성
        foreach (Flag flag in flags)
        {
            flag.time = UnityEngine.Random.Range(0, maxdist);
        }

        //NOTE 유저 확률분포 랜덤 생성
        // Vector2 userMean = new Vector2(0, 0);
        if (trivariate)
        {
            // userMean3 = new Vector3(UnityEngine.Random.Range(0, maxcount), UnityEngine.Random.Range(0, maxdist), UnityEngine.Random.Range(0, maxdist));
            userMean3 = new Vector3(UnityEngine.Random.Range(0, maxcount), UnityEngine.Random.Range(0, maxdist), UnityEngine.Random.Range(0, maxdist));
            maxGaus = (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, userMean3.x, userMean3.y, userMean3.z);
        }
        else
        {
            userMean = new Vector2(UnityEngine.Random.Range(0, maxcount), UnityEngine.Random.Range(0, maxdist));
            ((RectTransform)graph.GetChild(0)).anchoredPosition = new Vector3(userMean.x / maxcount, userMean.y / maxdist, 0) * 100;
            maxGaus = (float)GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, userMean.x, userMean.y, 0);
        }

        //유저 로그 생성 
        generateLog();
        updateFlagUI();
        RequestDecision();

    }

    void generateLog()
    {
        int count = 0;
        float dist;
        int visited;
        float time;
        while (count < logLen)
        {
            dist = UnityEngine.Random.Range(0, maxdist);
            visited = UnityEngine.Random.Range(0, (int)maxcount + 1);
            if (trivariate)
            {
                time = UnityEngine.Random.Range(0, maxdist);
                if (UnityEngine.Random.Range(0, 1.0f) < (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, visited, dist, time) / (maxGaus * 2))
                {
                    userLog3[count] = new Vector3(dist / maxdist, (float)visited / maxcount, time / maxdist);
                    count++;
                }
            }
            else
            {

                if (UnityEngine.Random.Range(0, 1.0f) < (float)GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, visited, dist, 0) / (maxGaus * 2))
                {
                    userLog[count] = new Vector2((float)visited / maxcount, dist / maxdist);
                    ((RectTransform)graph.GetChild(count + 2)).anchoredPosition = userLog[count] * 100;
                    count++;
                }
            }
        }
    }

    public void updateFlags(int dest)
    {
        if (dest >= 0)
        {
            if (destQfilled == destQSize)
            {
                flags[destQ.Dequeue()].visited--;
                destQfilled--;
            }

            destQ.Enqueue(dest);
            destQfilled++;
            flags[dest].visited++;
        }
        updateFlagUI();

    }

    public void updateFlagUI()
    {
        for (int i = 0; i < candidates.childCount; i++)
        {
            candidates.GetChild(i).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = flags[i].visited + "";
            if (trivariate)
            {
                candidates.GetChild(i).GetChild(2).GetChild(2).GetComponent<TMP_Text>().text = flags[i].time + "";
            }
            else
            {
                candidates.GetChild(i).GetChild(2).GetChild(1).transform.localPosition = new Vector3(-3, 3, 0);
                candidates.GetChild(i).GetChild(2).GetChild(2).gameObject.SetActive(false);
            }
        }

    }

    public bool showResult;
    public float showTime;

    float maxdist = 100 * Mathf.Sqrt(2);

    float maxcount;
    float thresh_count = 6;


    public static double GetBivariateGuassian(double muX, double sigmaX, double muY, double sigmaY, double x, double y, double rho = 0)
    {
        var sigmaXSquared = Math.Pow(sigmaX, 2);
        var sigmaYSquared = Math.Pow(sigmaY, 2);

        var dX = x - muX;
        var dY = y - muY;

        var exponent = -0.5;
        var normaliser = 2 * Math.PI * sigmaX * sigmaY;
        if (rho != 0)
        {
            normaliser *= Math.Sqrt(1 - Math.Pow(rho, 2));
            exponent /= 1 - Math.Pow(rho, 2);
        }

        var sum = Math.Pow(dX, 2) / sigmaXSquared;
        sum += Math.Pow(dY, 2) / sigmaYSquared;
        sum -= 2 * rho * dX * dY / (sigmaX * sigmaY);

        exponent *= sum;

        return Math.Exp(exponent) / normaliser;
    }

    public static double GetTrivariateGuassian(double muX, double sigmaX, double muY, double sigmaY, double muZ, double sigmaZ, double x, double y, double z)
    {
        var dX = x - muX;
        var dY = y - muY;
        var dZ = z - muZ;

        var exponent = Math.Pow(dX, 2) * sigmaY * sigmaZ + Math.Pow(dY, 2) * sigmaX * sigmaZ + Math.Pow(dZ, 2) * sigmaX * sigmaY;
        exponent *= (-1 / (2 * sigmaX * sigmaY * sigmaZ));
        var normaliser = Math.Pow(2 * Math.PI, 1.5) * Math.Sqrt(sigmaX * sigmaY * sigmaZ);

        return Math.Exp(exponent) / normaliser;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (getMean)
        {
            var count = (actionBuffers.ContinuousActions[0] + 1) / 2 * maxcount;
            var dist = (actionBuffers.ContinuousActions[1] + 1) / 2 * maxdist;

            if (trivariate)
            {
                var time = (actionBuffers.ContinuousActions[2] + 1) / 2 * maxdist;
                // rew = GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, count, dist, time) * 10000 / maxGaus;
                rew = Math.Sqrt(Math.Pow(userMean3.x - count, 2) + Math.Pow(userMean3.y - dist, 2) + Math.Pow(userMean3.z - time, 2)) / Math.Sqrt(Math.Pow(maxcount, 2) + Math.Pow(maxdist, 2) + Math.Pow(maxdist, 2));
                rew = 1 - rew;
                Debug.Log("rew: " + rew);
            }
            else
            {
                ((RectTransform)graph.GetChild(1)).anchoredPosition = new Vector3(count / maxcount, dist / maxdist, 0) * 100;
                rew = GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, count, dist, 0) / maxGaus;
                Debug.Log("rew: " + rew + "\nx: " + (actionBuffers.ContinuousActions[0] + 1) / 2 + " realX: " + (float)userMean.x / maxcount + "\n" +
                "y: " + (actionBuffers.ContinuousActions[1] + 1) / 2 + " realY: " + userMean.y / maxdist + "\n"
                );
            }
            AddReward((float)rew);

            if (showResult)
                Invoke("EndEpisode", showTime);
            else
                EndEpisode();
            return;
        }
        var action = actionBuffers.DiscreteActions[0];
        if (action != -1 && action < candidates.childCount)
        {
            going = true;

            curdest = action;
            if (!warp)
                owner.GetComponent<TrailGenerator>().goTo(action);
            candidates.GetChild(action).GetComponent<FlagColor>().red();


            float g = flagCount / destQSize;
            double dist = Vector3.Distance(owner.localPosition, flags[action].pos);
            double count = flags[action].visited;
            double time = flags[action].time;

            // float p_dist = MathF.Log((-dist + maxdist + 1) * (MathF.Exp(1) - 1) / (maxdist));
            // if (p_dist < 0.001f) p_dist = 0.001f;
            // float p_count = MathF.Log((-count + thresh_count + 1) * (MathF.Exp(1) - 1) / (thresh_count));
            // if (p_count < 0.001f) p_count = 0.001f;
            // rew = (p_dist * p_count);
            if (trivariate)
            {
                rew = GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, count, dist, time) / maxGaus;

            }
            else
            {
                rew = GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, count, dist, 0) / maxGaus;
            }

            AddReward((float)rew);

            if (debugReward)
            {
                Debug.Log("id: " + action + " #visited: " + flags[action].visited + " distance: " + dist + " reward: " + (rew));
            }

            if (warp)
            {
                if (showResult)
                    Invoke("EndEpisode", showTime);
                else
                    EndEpisode();
                return;
            }
            // curep++;
            // if (curep == epLength)
            //     EndEpisode();
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
                if (flags[i].visited < min)
                {
                    action = i;
                    min = flags[i].visited;
                }
            }
            discreteActionsOut[0] = action;
        }
    }
    public double rew;
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
