using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showSphere : MonoBehaviour
{
    public GameObject Sphere;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Sphere.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Sphere.SetActive(true);
            Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), GameManager.Instance.owner.GetComponent<Collider>(), false);
            GameManager.Instance.owner.GetComponent<JoinGroup>().sep();
        }
    }
}
