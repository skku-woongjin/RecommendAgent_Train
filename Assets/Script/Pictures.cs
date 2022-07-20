using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Pictures : MonoBehaviour
{
    public Sprite[] pics;
    // Start is called before the first frame update

    public void setPics()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            child.GetComponent<Image>().sprite = pics[i];
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
