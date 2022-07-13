using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catAnim : MonoBehaviour
{
    public Rigidbody rb;
    public Transform tr;
    public Animator anim;

    Quaternion lastRot;

    void Start()
    {
        lastRot = tr.rotation;
    }
    void FixedUpdate()
    {
        anim.SetFloat("speed", Vector3.SqrMagnitude(rb.velocity));
        // Debug.Log(anim.GetFloat("speed"));

        anim.SetFloat("turnSpd", Quaternion.Angle(lastRot, tr.rotation));
        bool stopping = (tr.GetComponent<IdleAgent>().state == IdleAgent.States.stop);
        if (anim.GetFloat("speed") < 1)
        {
            anim.SetFloat("speed", 1f);
        }
        if (tr.GetComponent<IdleAgent>().state == IdleAgent.States.outbound)
        {
            anim.SetFloat("speed", 3f);
        }

        anim.SetBool("Stopping", stopping);
        // Debug.Log(anim.GetFloat("turnSpd"));

        lastRot = tr.rotation;
    }
}
