using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OwnerController : MonoBehaviour
{
    public float speed;
    public float turnSpd;
    public Transform bound;
    public Transform dest;

    public Rigidbody rb;
    Vector3 dirVec;
    NavMeshAgent nav;
    NavMeshPath navPath;
    Vector3 direction;
    public bool randomMovement = false;


    void OnEnable()
    {
        nav = GetComponent<NavMeshAgent>();
        nav.updatePosition = true;
        nav.updateRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        bound.rotation = Quaternion.Euler(Vector3.zero);
        StopAllCoroutines();
        StartCoroutine("changedir");
        navPath = new NavMeshPath();
    }

    void Update()
    {
        if (nav.destination != null)
            nav.CalculatePath(nav.destination, navPath);
        int i = 1;
        while (i < navPath.corners.Length)
        {
            if (Vector3.Distance(transform.position, navPath.corners[i]) > 0.5f)
            {
                direction = navPath.corners[i] - transform.position;
                break;
            }
            i++;
        }

    }

    void FixedUpdate()
    {
        // if (randomMovement)
        // {
        //     transform.position += dirVec * speed * 0.1f;
        //     if (transform.position.x * transform.position.x > 2500 || transform.position.z * transform.position.z > 2500)
        //     {
        //         transform.position = new Vector3(0, transform.position.y, 0);
        //     }
        //     if (dirVec != Vector3.zero)
        //     {
        //         bound.rotation = Quaternion.Lerp(bound.rotation, Quaternion.LookRotation(dirVec), Time.deltaTime * turnSpd);
        //     }
        // }
        // if (nav.destination != null)
        //     nav.CalculatePath(nav.destination, navPath);

        // rb.AddForce((nav.nextPosition - transform.position).normalized * speed, ForceMode.VelocityChange);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(bound.forward * speed, ForceMode.VelocityChange);
            // transform.position += transform.forward * speed * 0.5f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += bound.forward * speed * -0.5f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            bound.Rotate(Vector3.up * turnSpd * 0.1f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            bound.Rotate(Vector3.up * turnSpd * -0.1f);
        }
        if (Vector3.Magnitude(rb.velocity) > 5)
        {
            rb.velocity *= 0.9f;
        }
    }

    IEnumerator changedir()
    {
        while (true)
        {
            if (randomMovement)
            {
                float walkRadius = Random.Range(20f, 50f);

                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                Vector3 finalPosition = hit.position;
                nav.SetDestination(finalPosition);
                dest.position = finalPosition + transform.up;
                // dirVec = new Vector3(Random.insideUnitSphere.normalized.x, 0, Random.insideUnitSphere.normalized.z);
                yield return new WaitForSecondsRealtime(Random.Range(10, 15));
            }
            yield return null;
        }
    }
}
