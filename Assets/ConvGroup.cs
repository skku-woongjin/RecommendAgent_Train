using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvGroup : MonoBehaviour
{
    public Material UDangTangMat;
    public Material CommonMat;
    public GameObject Sphere;
    public GameObject[] Users;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void hideSphere()
    {
        StartCoroutine("hideSphereCoroutine");
    }

    IEnumerator hideSphereCoroutine()
    {
        yield return new WaitForSecondsRealtime(1);
        Sphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
