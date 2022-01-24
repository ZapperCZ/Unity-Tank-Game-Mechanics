using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField] int[] slowmoMultiplier = new int[] {1,2,4,8};
    [SerializeField] int currentMultiplierIndex = 0;
    [SerializeField] bool slowmoActive;

    float slowmoHorizontalAxis = 0f;
    bool slowmoModifier = false;

    // Update is called once per frame
    void OnValidate()
    {
        HandleIndexOverflow();   
    }
    void Update()
    {
        GetInput();

        if (slowmoActive && slowmoModifier && slowmoAxis != 0)
        {
            if(slowmoAxis > 0)
            {
                currentMultiplierIndex++;
            }
            else
            {
                currentMultiplierIndex--;
            }
            HandleIndexOverflow();
        }
    }
    void GetInput()
    {
        slowmoHorizontalAxis = Input.GetAxis("Slowmotion Horizontal Axis");
        slowmoModifier = Input.GetButton("Slowmotion Modifier");
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
