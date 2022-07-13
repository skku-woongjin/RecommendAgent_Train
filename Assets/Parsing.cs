using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Parsing : MonoBehaviour
{
    private void Start()
    {
        List<Dictionary<string, object>> data_Dialog = CSVReader.Read("ConversationListData");

        for (int i = 0; i < data_Dialog.Count; i++)
        {
            Debug.Log(data_Dialog[i]["line"].ToString());
        }
    }
}
