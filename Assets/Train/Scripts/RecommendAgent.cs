using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using TMPro;
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

    public int logLen = 20;
    public bool debugReward;
    public bool showResult;
    public float showTime;
    public bool warp;
    public Transform candidates;

    public GameObject flagPrefab;
    public Transform owner;
    public Transform trails;

    [HideInInspector]
    public Flag[] flags;
    public int flagCount;
    public int destQSize;
    int destQfilled;
    Queue<int> destQ;

    public int worldSize;

    Vector3 userMean3;
    Vector3[] userLog3;

    double rew;

    void setRandomPosition()
    {
        for (int i = 0; i < flagCount; i++)
        {
            flags[i].pos = new Vector3(UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2), 0, UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2));
        }

        flags = flags.OrderBy(v => -v.pos.z).ThenBy(v => v.pos.x).ToArray<Flag>();

        int j = 0;
        foreach (Transform child in candidates)
        {
            child.transform.localPosition = flags[j].pos;
            j++;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (Vector3 vec in userLog3)
            sensor.AddObservation(vec);
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

        destQ = new Queue<int>();
        userLog3 = new Vector3[logLen];
        flags = new Flag[flagCount];
        for (int i = 0; i < flagCount; i++)
        {
            flags[i] = new Flag();
        }


        setRandomPosition();

    }

    public override void OnEpisodeBegin()
    {
        setRandomPosition();
        rew = 0;


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
        userMean3 = new Vector3(UnityEngine.Random.Range(0, maxcount), UnityEngine.Random.Range(0, maxdist), UnityEngine.Random.Range(0, maxdist));
        maxGaus = (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, userMean3.x, userMean3.y, userMean3.z);

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
            time = UnityEngine.Random.Range(0, maxdist);
            if (UnityEngine.Random.Range(0, 1.0f) < (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, visited, dist, time) / (maxGaus * 2))
            {
                userLog3[count] = new Vector3(dist / maxdist, (float)visited / maxcount, time / maxdist);
                count++;
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
            candidates.GetChild(i).GetChild(2).GetChild(2).GetComponent<TMP_Text>().text = flags[i].time + "";
        }
    }



    float maxdist = 100 * Mathf.Sqrt(2);

    float maxcount;


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
        var count = (actionBuffers.ContinuousActions[0] + 1) / 2 * maxcount;
        var dist = (actionBuffers.ContinuousActions[1] + 1) / 2 * maxdist;
        var time = (actionBuffers.ContinuousActions[2] + 1) / 2 * maxdist;

        rew = Math.Sqrt(Math.Pow(userMean3.x - count, 2) + Math.Pow(userMean3.y - dist, 2) + Math.Pow(userMean3.z - time, 2)) / Math.Sqrt(Math.Pow(maxcount, 2) + Math.Pow(maxdist, 2) + Math.Pow(maxdist, 2));
        rew = 1 - rew;

        if (debugReward)
            Debug.Log("rew: " + rew);


        AddReward((float)rew);

        if (showResult)
            Invoke("EndEpisode", showTime);
        else
            EndEpisode();

    }

}
