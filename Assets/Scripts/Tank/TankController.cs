using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject LeftSprocket;
    public GameObject RightSprocket;

    [HideInInspector] public Vector3 leftExitPos;
    [HideInInspector] public Vector3 rightExitPos;
    bool isSpaceOnLeft = true;
    bool isSpaceOnRight = true;

    public Vector3 CameraOffset;

    [SerializeField] Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if (Input.GetButtonDown("Use Key"))
        {
            Player.GetComponent<PlayerControllerManager>().SwitchController(true, this.isSpaceOnLeft, this.isSpaceOnRight);
            Debug.Log("Cock");
        }
    }

    void GetInput()
    {

    }
    public void EvaluateTrigger(double localX, bool isExiting)
    {
        if (localX < 0)  //Left
        {
            isSpaceOnLeft = isExiting;
        }
        else            //Right
        {
            isSpaceOnRight = isExiting;
        }
    }

}
