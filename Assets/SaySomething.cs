using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaySomething : MonoBehaviour
{
    public GameObject bubble;
    public Transform camTransform;

    void Start()
    {
    }
    public void say(string line)
    {
        bubble.GetComponentInChildren<TMP_Text>().text = line;
        bubble.transform.rotation = Quaternion.LookRotation(bubble.transform.position - camTransform.position);
        bubble.SetActive(true);
    }

}
