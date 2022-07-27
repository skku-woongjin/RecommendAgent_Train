using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AcceptQuest : MonoBehaviour
{
    public Button okButton;
    public Button noButton;
    public TMP_Text questAcceptText;
    // Start is called before the first frame update
    void Start()
    {
        okButton.onClick.AddListener(ConfirmQuest);
    }
    void ConfirmQuest(){

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
