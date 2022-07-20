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
        //dash
        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.AddForce(-bound.forward * speed * 10, ForceMode.VelocityChange);
            
            // transform.position += transform.forward * speed * 0.5f;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForce(bound.forward * speed * 10, ForceMode.VelocityChange);

            // transform.position += transform.forward * speed * 0.5f;
        }
        //move
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(bound.forward * speed, ForceMode.VelocityChange);
            // transform.position += transform.forward * speed * 0.5f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-bound.forward * speed, ForceMode.VelocityChange);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(bound.up * speed * 1000, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            rb.AddForce(bound.up * speed * 10000, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            rb.AddForce(-bound.up * speed * 10000, ForceMode.Impulse);
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
