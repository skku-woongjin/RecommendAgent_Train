using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class InputText : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField inputField;
   
    public ChatManager chatmanager;

    [System.Serializable]
    public class Question
    {//보내기 위한 질문
        public string question;
    }
    [System.Serializable]
    public class Answer
    {//받기 위한 answer
        public string answer;
    }

    string text;
    void Start()
    {   
        
    }

    

    // Update is called once per frame
    void Update()
    {

    }

    //VQA 질문을 대화창에 띄우기
    public void GetText()
    {
        if (inputField.text.Length > 0)
        {
            text = inputField.text;
            chatmanager.Chat(true, text, "나");
            inputField.text = "";
            StartCoroutine(webRequestGet());
        }


    }

    //VQA 대답
    IEnumerator webRequestGet()
    {
        //웹서버 url
        string url = "http://54.180.113.96:5000/vqa";//url 생성
        Question q = new Question();
        q.question = text;
        string question_data = JsonUtility.ToJson(q);
        var req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(question_data);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");




        //응답 데이터를 StreamReader로 받음
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while Sending: " + req.error);
        }
        else
        {
            chatmanager.Chat(false, req.downloadHandler.text, "타인");

        }


    }


}
