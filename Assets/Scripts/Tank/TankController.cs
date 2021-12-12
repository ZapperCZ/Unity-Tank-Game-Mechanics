using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    bool isSpaceOnLeft = true;
    bool isSpaceOnRight = true;

    [SerializeField] Transform Player;
    [SerializeField] Transform COM;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if (Input.GetButtonDown("UseKey"))
        {
            Player.GetComponent<PlayerControllerManager>().SwitchController(true, this.isSpaceOnLeft, this.isSpaceOnRight);
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
