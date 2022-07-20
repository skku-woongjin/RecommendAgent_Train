using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Is_Correct : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btn;//정답버튼
    public TMP_Text correct;//정답버튼 text

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void is_correct(){
        if(btn.interactable==false && correct.text=="이게 정답 맞지??"){
            Debug.Log("정답");
        }
    }
}
