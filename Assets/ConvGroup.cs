using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvGroup : MonoBehaviour
{
    public bool isbad;
    public Material UDangTangMat;
    public Material CommonMat;
    public GameObject Sphere;
    public GameObject[] Users;

    public void hideSphere()
    {
        StartCoroutine("hideSphereCoroutine");
    }

    IEnumerator hideSphereCoroutine()
    {
        yield return new WaitForSecondsRealtime(1);
        Sphere.SetActive(false);
    }

    public void join()
    {
        foreach (Transform u in transform)
        {
            if (u.GetComponent<SaySomething>() != null)
                u.GetComponent<SaySomething>().say("안녕");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
