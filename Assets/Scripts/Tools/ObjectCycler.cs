using UnityEngine;

public class ObjectCycler : MonoBehaviour
{
    [SerializeField] GameObject[] ObjectsToCycle;
    [SerializeField] bool useTrigger = true;    
    [SerializeField] bool useTimer = false;
    [SerializeField] bool useKey = false;
    [SerializeField] BoxCollider Trigger;
    [SerializeField] float timer = 10f;
    [SerializeField] string switchKey = "";
    bool prevUseTrigger;
    bool prevUseTimer;
    bool prevUseKey;

    int index = 0;
    int prevIndex;
    int actualIndex;

    void Awake()
    {
        prevUseTrigger = useTrigger;
        prevUseTimer = useTimer;
        prevUseKey = useKey;
        prevIndex = index;

        if (useKey)
        {
            try
            {
                Input.GetKey(switchKey);
            }
            catch
            {
                Debug.LogError(this.GetType().Name + " - " + this.name + " - \"" + switchKey + "\" is not a valid key");
                this.enabled = false;
                return;
            }
        }

        foreach(GameObject GObj in ObjectsToCycle)
        {
            GObj.SetActive(false);
        }
        ObjectsToCycle[index].SetActive(true);
    }

    void OnValidate()
    {
        ValidateInput();
    }
    void Update()
    {
        if (!Application.isPlaying)
            return;
        if (useKey)
        {
            if (Input.GetKeyDown(switchKey))
            {
                index++;
                HandleIndexOverflow();
            }
        }
        CycleObjects();
    }

    void CycleObjects()
    {
        if (prevIndex == index)
            return;

        actualIndex = index == 0 ? ObjectsToCycle.Length - 1 : index - 1;

        Debug.Log(actualIndex);
        ObjectsToCycle[actualIndex].SetActive(false);
        ObjectsToCycle[index].SetActive(true);

        prevIndex = index;
    }

    void ValidateInput()
    {
        if ((useTrigger == useTimer) || (useTrigger == useKey) || (useTimer == useKey))         //A switch for the booleans which assures that there is always only one option selected
        {
            if (useTrigger != prevUseTrigger)
            {
                prevUseTrigger = useTrigger = true;
                useTimer = prevUseTimer = false;
                useKey = prevUseKey = false;
            }
            if (useTimer != prevUseTimer)
            {
                prevUseTimer = useTimer = true;
                useTrigger = prevUseTrigger = false;
                useKey = prevUseKey = false;
            }
            if(useKey != prevUseKey)
            {
                prevUseKey = useKey = true;
                useTimer = prevUseTimer = false;
                useTrigger = prevUseTrigger = false;
            }
        }
    }
    void HandleIndexOverflow()
    {
        if (index > ObjectsToCycle.Length - 1)
        {
            index = 0;
        }
    }
}
