using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class catAnim : MonoBehaviour
{
    public Rigidbody rb;
    public Transform tr;
    public Animator anim;

    Quaternion lastRot;
    NavMeshAgent nav;
    void Start()
    {
        lastRot = tr.rotation;
        nav = tr.GetComponent<NavMeshAgent>();
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
        if (tr.GetComponent<NavMeshAgent>().enabled)
        {
            anim.SetFloat("speed", nav.speed);
        }

        anim.SetBool("Stopping", stopping);
        anim.SetBool("Say", tr.GetComponent<IdleAgent>().state == IdleAgent.States.say);
        // Debug.Log(anim.GetFloat("turnSpd"));

        lastRot = tr.rotation;
    }
}
