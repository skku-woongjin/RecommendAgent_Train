using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int questId;
    public int questActionIndex;
    public GameObject[] questObject;

    Dictionary<int, QuestData> questList;
    void Start()
    {
        questList = new Dictionary<int, QuestData>();
        GenerataData();
    }
    void GenerataData()
    {
        questList.Add(10, new QuestData("사람들과 대화하기", new int[] { 1000, 2000 }));
        questList.Add(20, new QuestData("증기를 찾아라", new int[] { 5000, 2000 }));
        questList.Add(30, new QuestData("퀘스트 올 클리어", new int[] { 0 }));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
