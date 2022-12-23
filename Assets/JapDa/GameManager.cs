using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float ownerEnergy;
    public IdleAgent idleAgent;
    public Transform owner;
    public ConvGroup curGroup;
    public Transform cam;
    public request req;
    public bool ingroup;

    private static GameManager gm;
    public static GameManager Instance
    {
        get
        {
            if (gm == null) Debug.LogError("Game Manager is null!");
            return gm;
        }
    }
    // Start is called before the first frame update
    private void Awake()
    {
        gm = this;
    }
}
