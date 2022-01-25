using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField] int[] slowmoMultiplier = new int[] {1,2,4,8};
    [SerializeField] int currentMultiplierIndex = 0;
    [SerializeField] bool slowmoActive;

    float slowmoHorizontalAxis = 0f;
    float slowmoVerticalAxis = 0f;
    bool slowmoModifier = false;

    // Update is called once per frame
    void OnValidate()
    {
        HandleIndexOverflow();   
    }
    void Update()
    {
        GetInput();


    }
    void GetInput()
    {
        slowmoHorizontalAxis = Input.GetAxis("Slowmotion Horizontal Axis");
        slowmoModifier = Input.GetButton("Slowmotion Modifier");

        if (Input.GetButtonDown("Slowmotion Toggle"))
        {
            slowmoActive = !slowmoActive;
        }
        if (slowmoModifier)
        {
            if (slowmoActive && slowmoHorizontalAxis != 0)
            {
                if (slowmoHorizontalAxis > 0)           //Player pressed left
                {
                    currentMultiplierIndex++;
                }
                else
                {
                    currentMultiplierIndex--;           //Player pressed right
                }
                HandleIndexOverflow();
            }
        }
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
