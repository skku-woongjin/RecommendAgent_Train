using UnityEngine;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class IdleAgent : Agent
{
    Rigidbody m_AgentRb;

    public Material blueMat;
    public Material purpleMat;
    public Material redMat;
    public Material greenMat;
    public Material yellowMat;
    public enum States
    {
        rand,
        inte,
        stop,
        bound,
        avoid,
        outbound,
        say
    }

    public States state;

    public bool colliding = false;

    bool inbound;
    bool interested;

    public float turnSpeed = 300;
    public float moveSpeed = 2;

    public float rew;
    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        inteStop = stop(true);
        ChangeDir = changedir();
        rend = gameObject.GetComponentInChildren<Renderer>();
    }

    public OwnerController ownerController;
    IEnumerator ChangeDir;

    public override void OnEpisodeBegin()
    {
        Physics.IgnoreLayerCollision(3, 8);
        StopAllCoroutines();
        transform.localPosition = new Vector3(-3.5f, 1.02f, 3.8f);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        Random.InitState(50);
        StartCoroutine(ChangeDir);
        ownerController.enabled = false;
        owner.position = new Vector3(0, 1.55f, 1.84f);
        ownerController.enabled = true;
        m_AgentRb.velocity = Vector3.zero;
        gameObject.GetComponentInChildren<Renderer>().material = blueMat;
        StartCoroutine("StopStop");
        state = States.rand;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation((int)state - 4);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);
        sensor.AddObservation((owner.position.x - transform.position.x) / 50f);
        sensor.AddObservation((owner.position.z - transform.position.z) / 50f);
        // sensor.AddObservation(owner.position.x);
        // sensor.AddObservation(owner.position.z);
    }

    public Transform owner;
    Vector3 dirVec;
    float autoTurnSpeed = 150;
    float autoMoveSpeed = 0.15f;
    IEnumerator inteStop;

    Vector3 removY(Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }
    public void MoveAgent(ActionBuffers actionBuffers) // 매 프레임 호출 
    {
        if (state == States.say)
            return;
        switch (state)
        {
            case States.rand:

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dirVec), Time.deltaTime * autoTurnSpeed * 0.01f);
                if (m_AgentRb.velocity.sqrMagnitude > 1f) //최대속도 설정
                {
                    m_AgentRb.velocity *= 0.95f;
                }

                break;

            case States.inte:

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(removY(interestingObj.position - transform.position)), Time.deltaTime * autoTurnSpeed * 0.03f);
                m_AgentRb.AddForce(transform.forward * autoMoveSpeed, ForceMode.VelocityChange);
                if (Vector3.SqrMagnitude(interestingObj.position - transform.position) < 20)
                {
                    if (Vector3.SqrMagnitude(interestingObj.position - transform.position) < 20 && state != States.stop && !decel)
                    {
                        StartCoroutine(inteStop);
                    }

                }
                else
                {
                    if (m_AgentRb.velocity.sqrMagnitude < 2f) //최소속도 설정
                    {
                        m_AgentRb.velocity *= 1.05f;
                    }
                }
                if (m_AgentRb.velocity.sqrMagnitude > 3f) //최대속도 설정
                {
                    m_AgentRb.velocity *= 0.95f;
                }

                break;

            case States.avoid:
                float rot = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
                Vector3 rotateDir = -transform.up * rot;
                if (rotateDir.sqrMagnitude > 0.1)
                {
                    transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
                }

                m_AgentRb.AddForce(transform.forward * moveSpeed * 0.2f, ForceMode.VelocityChange);
                if (m_AgentRb.velocity.sqrMagnitude > 1f) //최대속도 설정
                {
                    m_AgentRb.velocity *= 0.95f;
                }

                if (m_AgentRb.velocity.sqrMagnitude < 0.5f) //최소속도 설정
                {
                    m_AgentRb.velocity *= 1.05f;
                }
                // }

                break;

            case States.bound:
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(removY(owner.position - transform.position)), Time.deltaTime * turnSpeed * 0.01f);
                m_AgentRb.AddForce(transform.forward * moveSpeed * .2f, ForceMode.VelocityChange);
                if (m_AgentRb.velocity.sqrMagnitude > 1.5f) //최대속도 설정
                {
                    m_AgentRb.velocity *= 0.95f;
                }

                if (m_AgentRb.velocity.sqrMagnitude < 0.5f) //최소속도 설정
                {
                    m_AgentRb.velocity *= 1.05f;
                }
                break;
        }


        if (state == States.outbound) //비상!!!
        {
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<NavMeshAgent>().SetDestination(owner.position);
            // float rot = Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
            // Vector3 rotateDir = -transform.up * rot;
            // if (rotateDir.sqrMagnitude > 0.1)
            // {
            //     transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
            // }

            // // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(removY(owner.position + owner.right * 1.5f - transform.position)), Time.deltaTime * autoTurnSpeed);
            // m_AgentRb.AddForce(transform.forward * 3 * moveSpeed, ForceMode.VelocityChange);
            // if (m_AgentRb.velocity.sqrMagnitude > owner.GetComponent<Rigidbody>().velocity.sqrMagnitude * 1.5) //최대속도 설정
            // {
            //     m_AgentRb.velocity *= 0.95f;
            // }
            // if (m_AgentRb.velocity.sqrMagnitude < 6) //최소속도 설정
            // {
            //     m_AgentRb.velocity *= 1.5f;
            // }

        }
        else
        {
            if (state != States.stop && state != States.say)
                m_AgentRb.AddForce(transform.forward * autoMoveSpeed * 2.4f, ForceMode.VelocityChange);
        }
        // if (boundAgent)
        // {
        //     var continuousActions = actionBuffers.ContinuousActions;

        //     rot = continuousActions[2];
        //     spd = continuousActions[0];

        //     dirToGo = Vector3.zero;
        //     rotateDir = Vector3.zero;

        //     var forward = Mathf.Clamp(spd, -1f, 1f);
        //     var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
        //     var rotate = Mathf.Clamp(rot, -1f, 1f);

        // rotateDir = -transform.up * rotate;
        // if (rotateDir.sqrMagnitude > 0.1)
        // {
        //     transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        // }
        //     dirToGo = transform.forward * (forward);

        //     m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);

        //     if (m_AgentRb.velocity.sqrMagnitude > 10f) //최대속도 설정
        //     {
        //         m_AgentRb.velocity *= 0.95f;
        //     }

        //     // if (m_AgentRb.velocity.sqrMagnitude < 10f) //최소속도 설정
        //     // {
        //     //     m_AgentRb.AddForce(transform.forward * moveSpeed * 5, ForceMode.VelocityChange);
        //     // }
        // }

    }
    public override void OnActionReceived(ActionBuffers actionBuffers) { MoveAgent(actionBuffers); }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[0] = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[0] = 1;
        }
        if (state == States.avoid)
        {

            if (obstacle == null)
                endObst();
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(removY(-obstacle.position + transform.position)), Time.deltaTime * turnSpeed * 0.01f);
            }
        }
        // if (state == States.outbound)
        // {
        //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(removY(owner.position + owner.right * 1.5f - transform.position)), Time.deltaTime * autoTurnSpeed);
        // }
    }

    #region collision
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("ground"))
        {
            colliding = true;

            if (collision.collider.CompareTag("Player"))
            {
                state = States.avoid;
                ObstAgent(collision.transform);
            }

            if (state != States.avoid && state != States.outbound && state != States.inte)
            {
                ObstAgent(collision.transform);
            }
        }


    }

    private void OnCollisionExit(Collision other)
    {
        colliding = false;
        if (obstacle != null && other.gameObject == obstacle.gameObject)
        {
            endObst();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("bound"))
        {
            OutBoundAgent();
            // AddReward(-1f);
            // EndEpisode();
        }
        else if (other.gameObject.CompareTag("aibound2"))
        {
            BoundAgent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bound"))
        {

            BoundAgent();
            // Physics.IgnoreLayerCollision(0, 3, false);
        }
        else if (other.gameObject.CompareTag("aibound2"))
        {
            inbound = true;
            if (state == States.bound)
            {
                endBoundAgent();
            }
        }
    }

    #endregion

    Renderer rend;
    void setMat()
    {
        if (state != States.outbound)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }
        switch (state)
        {
            case States.rand:
                rend.material = blueMat;
                break;
            case States.inte:
                rend.material = greenMat;
                break;
            case States.avoid:
                rend.material = yellowMat;
                break;
            case States.bound:
                rend.material = purpleMat;
                break;
            case States.outbound:
                rend.material = redMat;
                break;
        }
    }

    #region interest
    public Transform interestingObj;
    public void interest()
    {
        interested = true;
        if (state <= States.inte)
        {
            state = States.inte;
            setMat();
        }
    }

    public void endInterest()
    {
        interested = false;
        ObstAgent(interestingObj);
        interestingObj = null;
        setMat();
    }

    #endregion

    public GameObject QuoteCanv;
    public void say()
    {
        stopStart();
        transform.rotation = Quaternion.LookRotation(-removY(transform.position - GameManager.Instance.owner.position));
        QuoteCanv.transform.rotation = Quaternion.LookRotation(QuoteCanv.transform.position - GameManager.Instance.cam.position);
        QuoteCanv.SetActive(true);

        state = States.say;
    }

    public void endSay()
    {
        state = States.rand;
        obstacle = null;
        if (interested)
        {
            interest();
        }
        if (!inbound)
        {
            BoundAgent();
        }
        setMat();
    }

    #region boundAgent

    void BoundAgent()
    {
        inbound = false;
        if (state <= States.bound || state == States.outbound)
        {
            if (state == States.stop || decel)
            {
                stopEnd();
            }
            state = States.bound;
            setMat();
        }
    }

    void endBoundAgent()
    {
        state = States.rand;
        if (interested)
        {
            interest();
        }
        setMat();
    }

    #endregion

    #region outboundAgent
    void OutBoundAgent()
    {
        if (state == States.say) return;
        if (state == States.stop || decel)
        {
            stopEnd();
        }
        // Physics.IgnoreLayerCollision(0, 3);

        state = States.outbound;
        setMat();
    }


    #endregion
    //NOTE Avoid
    #region avoid 

    public Transform obstacle;
    public void ObstAgent(Transform obs)
    {
        if (state == States.outbound || state == States.say)
        {
            return;
        }
        if (state == States.avoid || decel)
        {
            stopEnd();
        }
        obstacle = obs;
        state = States.avoid;
        setMat();
    }

    public void endObst()
    {
        state = States.rand;
        obstacle = null;
        if (interested)
        {
            interest();
        }
        if (!inbound)
        {
            BoundAgent();
        }
        setMat();
    }

    #endregion

    #region update

    void FixedUpdate()
    {
        rew = GetCumulativeReward();
        if (colliding)
        {
            AddReward(-0.0002f);
        }
        if (state == States.outbound || state == States.avoid)
        {
            RequestDecision();
        }
        else
        {
            AddReward(0.0002f);
        }
        RequestAction();

        if (state != States.outbound && state != States.say && Vector3.SqrMagnitude(owner.position - transform.position) > 100)
        {
            OutBoundAgent();
        }
    }

    IEnumerator changedir()
    {
        while (true)
        {
            if (state == States.rand)
            {
                dirVec = new Vector3(Random.insideUnitSphere.normalized.x, 0, Random.insideUnitSphere.normalized.z);
            }
            autoMoveSpeed = Random.Range(0.1f, moveSpeed);
            autoTurnSpeed = Random.Range(50, turnSpeed);
            yield return new WaitForSecondsRealtime(Random.Range(1, 8));

        }
    }

    #endregion

    #region stop
    public bool decel;

    IEnumerator stop(bool inte)
    {
        // gameObject.GetComponentInChildren<Renderer>().material = grayMat;
        StopCoroutine(ChangeDir);
        ChangeDir = changedir();
        decel = true;
        while (m_AgentRb.velocity.sqrMagnitude > 0.001f)
        {
            autoMoveSpeed *= 0.99f;
            yield return new WaitForFixedUpdate();
        }
        decel = false;
        stopStart();
        if (inte)
        {
            yield return new WaitForSecondsRealtime(Random.Range(3f, 4.5f));
        }
        else
            yield return new WaitForSecondsRealtime(Random.Range(1.5f, 4));
        stopEnd();
        if (inte)
        {
            endInterest();
        }
        // gameObject.GetComponentInChildren<Renderer>().material = blueMat;
    }

    IEnumerator JustStop;
    IEnumerator StopStop()
    {
        while (true)
        {
            if (state != States.avoid && !decel)
            {
                yield return new WaitForSecondsRealtime(Random.Range(4.5f, 15));
                JustStop = stop(false);
                if (state == States.rand)
                    StartCoroutine(JustStop);
            }
            yield return new WaitForFixedUpdate();
        }
    }


    void stopStart()
    {
        state = States.stop;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    void stopEnd()
    {
        if (state == States.stop)
            state = States.rand;
        decel = false;
        if (inteStop != null)
            StopCoroutine(inteStop);
        if (JustStop != null)
            StopCoroutine(JustStop);
        inteStop = stop(true);
        StartCoroutine(ChangeDir);
    }

    #endregion
}
