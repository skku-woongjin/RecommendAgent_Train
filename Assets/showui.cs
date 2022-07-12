using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showui : MonoBehaviour
{
    public GameObject canv;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canv.transform.rotation = Quaternion.Inverse(other.transform.rotation);
            canv.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canv.SetActive(false);
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
