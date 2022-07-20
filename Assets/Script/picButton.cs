using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class picButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite X_Img;
    public GameObject X;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void clicked()
    {

        transform.parent.GetComponent<Choose_Pic>().can_press(transform.GetSiblingIndex());
        if (transform.parent.GetComponent<Choose_Pic>().checkAns)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            if (X.activeSelf)
            {
                X.SetActive(false);
            }
            else
            {
                X.SetActive(true);
            }
        }
    }
}
