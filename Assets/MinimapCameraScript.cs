using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
public class MinimapCameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject minimap_panel;
    private Button mapButton;
    private int checkMapOpen;
    void Start()
    {
        checkMapOpen = 1;
        mapButton = GameObject.FindGameObjectWithTag("MapOpenBtn").GetComponent<Button>();
        mapButton.onClick.AddListener(mapOnOff);
        minimap_panel = GameObject.Find("MinimapPanel");
        minimap_panel.SetActive(false);
    }
    void mapOnOff(){
        if(checkMapOpen == 0){
            checkMapOpen = 1;
            minimap_panel.SetActive(true);
        }
        else{
            checkMapOpen = 0;
            minimap_panel.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
