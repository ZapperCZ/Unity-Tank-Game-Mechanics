using System;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField] int[] slowmoMultiplier = new int[] {1,2,4,8};
    [SerializeField] int currentMultiplierIndex = 1;
    [SerializeField] bool slowmoActive;
    [SerializeField] float inputMsDelay = 200f;      //The delay on control input in ms    

    float slowmoHorizontalAxis = 0f;
    bool slowmoModifier = false;
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
        else
        {
            Time.timeScale = 1;
        }
    }
    void GetInput()
    {
        slowmoHorizontalAxis = Input.GetAxis("Slowmotion Horizontal Axis");
        slowmoModifier = Input.GetButton("Slowmotion Modifier");

        if (slowmoModifier)
        {
            if (Input.GetButtonDown("Slowmotion Toggle"))
            {
                slowmoActive = !slowmoActive;
            }
            if (slowmoActive && slowmoHorizontalAxis != 0)
            {
                if (slowmoHorizontalAxis > 0)           //Player pressed left
                {
                    increaseSlowmo = true;
                }
                else                                    //Player pressed right
                {
                    decreaseSlowmo = true;
                }
                if (internalTimer > inputMsDelay / 1000)
                {
                    internalTimer = 0;
                }
            }
        }
        if (internalTimer > inputMsDelay / 1000)      //Only gets run once the internal timer has reached the input delay value
        {
            currentMultiplierIndex += Convert.ToInt32(increaseSlowmo);
            currentMultiplierIndex -= Convert.ToInt32(decreaseSlowmo);
            increaseSlowmo = false;
            decreaseSlowmo = false;

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
