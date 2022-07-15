using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaySomething : MonoBehaviour
{
    public GameObject bubble;
    public Transform camTransform;

    Image bubbleImg;

    void Start()
    {
        bubbleImg = bubble.GetComponentInChildren<Image>();
        say("개새끼");
    }
    public void say(string line)
    {
        bubble.GetComponentInChildren<TMP_Text>().text = line;
        GetComponent<request>().sendReq(line);
        bubble.SetActive(true);
        StartCoroutine("fadeout");
    }

    private void Update()
    {
        if (bubble.activeSelf)
        {
            bubble.transform.rotation = Quaternion.LookRotation(bubble.transform.position - camTransform.position);
        }
    }

    IEnumerator fadeout()
    {
        Color c;
        yield return new WaitForSecondsRealtime(2f);
        while (bubbleImg.color.a > 0.01f)
        {
            c = bubbleImg.color;
            c.a -= 0.02f;
            bubbleImg.color = c;
            yield return new WaitForFixedUpdate();
        }
        bubble.SetActive(false);
        bubbleImg.color = Color.white;
    }

}
