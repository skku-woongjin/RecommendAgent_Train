using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iconSen : MonoBehaviour
{
    public IdleAgent agent;
    private void FixedUpdate()
    {
        transform.position = agent.transform.position;
        transform.position += Vector3.up * 30;
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
