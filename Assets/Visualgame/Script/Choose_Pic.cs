using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System;


public class Choose_Pic : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text answer;
    public int REALAns;  //정답
    public Image X_img;
    int curAns; //내가 고른 답

    public InputText inputText;

    public bool checkAns;

    public ChatManager chatmanager;
    public Button btn;//정답&이거 정답맞지?버튼
    public Transform hearts;

    public int wrongCount = 0;
    int i = 0;
    void Start()
    {
        Debug.Log("start 부름");
        getImgs();

    }

    public void print_caption()
    {
        //Debug.Log("caption: "+caption);
        string text = "내가 생각한 그림은\n<color=blue>" + caption + "</color>\n이야. \n이제부턴 영어로 질문해보자!";
        chatmanager.Chat(false, text, "타인");
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)chatParent);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region getimg
    public void getImgs()
    {
        Debug.Log("get_image 부름");
        StartCoroutine(GetRequest("http://54.180.113.96:5000/caption"));
        StartCoroutine(GetRequest_reload("http://54.180.113.96:5000/reload"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    decode(webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    IEnumerator GetRequest_reload(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();


            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("reload: Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("reload: HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("reload: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }

    public string caption;

    // Start is called before the first frame update
    void decode(string str)
    {
        Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
        string[] sep = str.Split('*');
        REALAns = Int32.Parse(sep[0]);
        caption = sep[1];
        print_caption();
        str = sep[2];
        str = regex.Replace(str, " ");
        str = str.Replace("b'", "");
        str = str.Replace("'", "");
        string[] imgs = str.Split('\n', ' ', '\r');

        int i = 0;

        foreach (string img in imgs)
        {
            if (img.Length < 3)
                continue;
            byte[] imageBytes = Convert.FromBase64String(img);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            GetComponent<Pictures>().pics[i] = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            i++;
        }
        Shuffle(GetComponent<Pictures>().pics);
        GetComponent<Pictures>().setPics();
    }
    private System.Random _random = new System.Random();
    void Shuffle(Sprite[] array)
    {
        int p = array.Length;
        for (int n = p - 1; n > 0; n--)
        {
            //swap r and n
            int r = _random.Next(0, n);
            Sprite t = array[r];
            array[r] = array[n];
            array[n] = t;
            if (r == REALAns)
            {
                REALAns = n;
            }
            else if (n == REALAns)
            {
                REALAns = r;
            }
        }
    }

    #endregion

    public void change_text()
    {
        if (i == 1)
        {
            i = 0;
            return;
        }
        answer.text = "이게 정답 맞지??";
        checkAns = true;
        btn.interactable = false;

    }
    public void can_press(int idx)
    {
        transform.GetChild(curAns).GetChild(1).gameObject.SetActive(false);
        curAns = idx;
        if (btn.interactable == false)
        {
            btn.interactable = true;

        }
    }

    //정답일거같은 이미지 pick
    public void choose()
    {
        //정답맞는지 확인
        if (answer.text.Equals("이게 정답 맞지??"))
        {
            chatmanager.Chat(true, "이게 정답 맞지??", "나");
            if (curAns == REALAns)
            {
                chatmanager.Chat(false, "YES!!!!!", "타인");

                //다음 라운드로 이동(새로운 랜덤이미지들)
                StartCoroutine(ReloadScene());
            }
            else
            {
                chatmanager.Chat(false, "No", "타인");
                if (hearts.GetChild(wrongCount).gameObject.activeSelf)
                {
                    hearts.GetChild(wrongCount).gameObject.SetActive(false);
                    wrongCount++;
                    if (wrongCount == 3)
                    {
                        chatmanager.Chat(false, "Game Over", "타인");
                        SceneManager.LoadScene(0);
                        return;
                    }
                }
                //틀렸을 떄 X로 바꾸기
                transform.GetChild(curAns).transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(curAns).GetChild(0).gameObject.SetActive(true);
            }

            btn.interactable = true;
            answer.text = "정답";
            checkAns = false;
            i = 1;
        }
    }
    public Transform chatParent;
    IEnumerator ReloadScene()
    {
        yield return new WaitForSecondsRealtime(1);
        foreach (Transform child in chatParent)
        {
            Destroy(child.gameObject);
            transform.GetChild(curAns).transform.GetChild(1).gameObject.SetActive(false);

        }
        foreach (Transform child in transform)
        {
            child.GetChild(0).gameObject.SetActive(false);
        }
        getImgs();
    }


    // public void Acive_X()
    // {
    //     if (answer.text.Equals("정답") && btn.interactable == true)
    //     {
    //         // X_img.enabled = true;
    //     }
    // }
}

