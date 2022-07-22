using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Chat
{
    public string chat;
}
public class Quest
{
    public string keywords;
}
public class request : MonoBehaviour
{
    public bool ishate = false;
    void Start()
    {
        StartCoroutine(UploadKeyword("jump*monster*park"));
    }

    // IEnumerator getRequest(string uri)
    // {
    //     UnityWebRequest uwr = UnityWebRequest.Get(uri);
    //     yield return uwr.SendWebRequest();
    //     if (uwr.result == UnityWebRequest.Result.ConnectionError)
    //     {
    //         Debug.Log("Error While Sending: " + uwr.error);
    //     }
    //     else
    //     {
    //         Debug.Log("Received: " + uwr.downloadHandler.text);
    //     }
    // }

    public IEnumerator Upload(System.Action<bool> callback, string line)
    {
        Chat body = new Chat();
        body.chat = line;
        string bodyData = JsonUtility.ToJson(body);
        Debug.Log(bodyData);
        // var postData = System.Text.Encoding.UTF8.GetBytes(bodyData);
        var req = new UnityWebRequest("http://3.37.88.40:5000/prediction", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(bodyData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            string res = req.downloadHandler.text;
            if (res.Contains("hate"))
            {
                ishate = true;
            }
            else
            {
                ishate = false;
            }
            callback(ishate);
        }
    }
    public IEnumerator UploadKeyword(string line)
    {
        //line -> 보낼 데이터
        //string actionKeyword = "jump/fly/walk/go/play/find";
        //string locationKeyword = "jurassic park/game machine/planet land/play ground/castle";
        //string objectKeyword = "";

        Quest body = new Quest();
        body.keywords = line;
        string bodyData = JsonUtility.ToJson(body);
        Debug.Log(bodyData);
        // var postData = System.Text.Encoding.UTF8.GetBytes(bodyData);
        var req = new UnityWebRequest("http://54.180.107.240:5000/generator", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(bodyData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            string res = req.downloadHandler.text;
        }
    }
}
