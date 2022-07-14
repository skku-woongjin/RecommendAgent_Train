using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showui : MonoBehaviour
{
    public GameObject ButtonCanv;
    public GameObject UdangCanv;

    public GameObject sphere;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.curGroup = GetComponent<ConvGroup>();
            ButtonCanv.transform.rotation = Quaternion.LookRotation(ButtonCanv.transform.position - other.transform.GetComponentInChildren<Camera>().transform.position);
            ButtonCanv.SetActive(true);
            if (!sphere.activeSelf)
            {
                sphere.SetActive(true);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ButtonCanv.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
