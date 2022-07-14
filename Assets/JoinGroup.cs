using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGroup : MonoBehaviour
{
    public Transform pet;

    public Transform T;

    public void warn()
    {
        GameManager.Instance.idleAgent.say();
    }
    public void join()
    {
        Transform t = GameManager.Instance.curGroup.transform.GetChild(0);
        transform.SetParent(t.parent);
        t.parent.GetComponent<ConvGroup>().join();
        Physics.IgnoreCollision(GetComponent<Collider>(), t.parent.GetComponent<Collider>(), true);
        Physics.IgnoreCollision(pet.GetComponent<Collider>(), t.parent.GetComponent<Collider>(), true);
        GetComponent<Rigidbody>().AddForce(transform.forward * 3000, ForceMode.Impulse);
        t.parent.GetComponent<ConvGroup>().hideSphere();
        // Physics.IgnoreCollision(GetComponent<Collider>(), t.parent.GetComponent<Collider>(), false);
    }

    public void sep()
    {
        transform.SetParent(transform.parent.parent);
    }

}
