using System;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField] int[] slowmoMultiplier = new int[] {1,2,4,8};
    [SerializeField] int currentMultiplierIndex = 1;
    [SerializeField] bool slowmoActive;
    [SerializeField] float inputMsDelay = 200f;      //The delay on control input in ms    

    float slowmoHorizontalAxis = 0f;
    bool slowmoPrevState = false;
    bool increaseSlowmo = false;
    bool decreaseSlowmo = false;
    float internalTimer = 0f;

    // Update is called once per frame
    void OnValidate()
    {
        HandleIndexOverflow();   
    }
    void Update()
    {
        internalTimer = internalTimer + Time.deltaTime / Time.timeScale;        //A system that always increases the time uniformly, no matter the FPS or time scale

        GetInput();

        if (slowmoActive)
        {
            Time.timeScale = 1f / slowmoMultiplier[currentMultiplierIndex];
        }
        else if(slowmoPrevState != slowmoActive)
        {
            Time.timeScale = 1f;
        }
        slowmoPrevState = slowmoActive;
    }
    void GetInput()
    {

        slowmoHorizontalAxis = 0;

        if (Input.GetButton("Slowmotion Modifier"))
        {
            if (Input.GetButtonDown("Slowmotion Horizontal Axis"))
            {
                slowmoHorizontalAxis = Input.GetAxis("Slowmotion Horizontal Axis");
            }
            if (Input.GetButtonDown("Slowmotion Toggle"))
            {
                slowmoActive = !slowmoActive;
            }
            if (slowmoHorizontalAxis != 0)
            {
                if (slowmoHorizontalAxis > 0)           //Player pressed left
                {
                    slowmoActive = true;
                    currentMultiplierIndex++;
                }
                else                                    //Player pressed right
                {
                    slowmoActive = true;
                    currentMultiplierIndex--;
                }
            }
        }
        HandleIndexOverflow();
    }
    void HandleIndexOverflow()
    {
        if(currentMultiplierIndex > slowmoMultiplier.Length-1)
        {
            currentMultiplierIndex = slowmoMultiplier.Length - 1;
        }
        if(currentMultiplierIndex < 0)
        {
            currentMultiplierIndex = 0;
        }
    }
}
