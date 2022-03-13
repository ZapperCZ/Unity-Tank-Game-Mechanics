using UnityEngine;

public class ObjectCycler : MonoBehaviour
{
    [SerializeField] GameObject[] ObjectsToCycle;
    [SerializeField] bool createInstances = false;          //A more resource intensive way to cycle the objects, however allows th
    [SerializeField] bool useTrigger = true;    
    [SerializeField] bool useTimer = false;
    [SerializeField] bool useKey = false;
    [SerializeField] BoxCollider Trigger;
    [SerializeField] float switchingTime = 5f;
    [SerializeField] string switchKey = "";
    bool prevUseTrigger;
    bool prevUseTimer;
    bool prevUseKey;
    GameObject prevInstance;

    int index = 0;
    int prevIndex = -1;
    int actualIndex;
    float internalTimer = 0;
    GameObject currInstance;

    void Start()
    {
        prevUseTrigger = useTrigger;
        prevUseTimer = useTimer;
        prevUseKey = useKey;

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
        //ObjectsToCycle[index].SetActive(true);
    }

    void OnValidate()
    {
        ValidateInput();
    }
    void Update()
    {
        if (useKey)
        {
            if (Input.GetKeyDown(switchKey))
            {
                index++;
                HandleIndexOverflow();
            }
        }
        else if (useTimer)
        {
            if (internalTimer < switchingTime)
                internalTimer += Time.deltaTime;
            else
            {
                index++;
                HandleIndexOverflow();
                internalTimer = 0;
            }
        }
        CycleObjects();
    }       

    void CycleObjects()
    {
        if (prevIndex == index)
            return;

        actualIndex = index == 0 ? ObjectsToCycle.Length - 1 : index - 1;

        if (createInstances)
        {
            if(prevInstance!=null)
                Destroy(prevInstance);
            currInstance = Instantiate(ObjectsToCycle[index]);
            currInstance.transform.parent = this.transform;
            currInstance.transform.localScale = ObjectsToCycle[index].transform.localScale;
            currInstance.transform.localRotation = ObjectsToCycle[index].transform.localRotation;
            currInstance.transform.localPosition = Vector3.zero;
            currInstance.SetActive(true);
            prevInstance = currInstance;
        }
        else
        {
            ObjectsToCycle[actualIndex].SetActive(false);
            ObjectsToCycle[index].SetActive(true);
        }

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
        if(switchingTime == 0)
        {
            switchingTime = 0.1f;
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
