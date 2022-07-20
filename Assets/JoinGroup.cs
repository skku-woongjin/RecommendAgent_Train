using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGroup : MonoBehaviour
{
    public Transform pet;

    public Transform T;

    Vector3 removY(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }

    public void warn()
    {
        GameManager.Instance.idleAgent.say();
    }
    public void join()
    {
        if (!GameManager.Instance.curGroup.isbad)
            GameManager.Instance.idleAgent.endObst();
        GameManager.Instance.ingroup = true;
        Transform t = GameManager.Instance.curGroup.transform.GetChild(0);
        transform.SetParent(t.parent);
        transform.rotation = Quaternion.LookRotation(removY(-transform.position + t.position));
        Physics.IgnoreCollision(GetComponent<Collider>(), t.parent.GetComponent<Collider>(), true);
        Physics.IgnoreCollision(pet.GetComponent<Collider>(), t.parent.GetComponent<Collider>(), true);
        GetComponent<Rigidbody>().AddForce(transform.forward * 3000, ForceMode.Impulse);
        t.parent.GetComponent<ConvGroup>().hideSphere();
        t.parent.GetComponent<ConvGroup>().join();
        // Physics.IgnoreCollision(GetComponent<Collider>(), t.parent.GetComponent<Collider>(), false);
    }

    public void sep()
    {
        if (GameManager.Instance.curGroup.isbad)
            GameManager.Instance.idleAgent.endObst();
        transform.SetParent(transform.parent.parent);
        GameManager.Instance.ingroup = false;
    }

}
