using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectFreezer : MonoBehaviour
{
    [SerializeField] bool freezeAllChildren = true;                 //Whether the children of the parent should be frozen as well
    [SerializeField] bool freeze = false;                           //Set the current state
    [SerializeField] bool unfreezeOnPlayMode = true;                //Whether the object(s) should be unfrozen when entering playmode
    [SerializeField] float unfreezeDelay = 1f;                      //Delay after changing the state
    [SerializeField] bool useDelayBetweenObjectUnfreeze = true;     //Use delay between objects while changing their state
    [SerializeField] float objectFreezeDelay = 0.05f;               //The delay between objects while changing their state
    [SerializeField] int minChildAmount = 1;                        //The minimum amount of children an object has to have for the children to be changed as well 

    float _unfreezeDelay = float.MaxValue;
    bool valuesChanged = false;

    private void Awake()
    {
        _unfreezeDelay = unfreezeDelay;
        valuesChanged = true;
    }

    private void OnValidate()
    {
        valuesChanged = true;
    }

    void Update()
    {
        if (Application.isPlaying && unfreezeOnPlayMode)
        {
            if (_unfreezeDelay >= 0)        //Timer hasn't reached 0
            {
                _unfreezeDelay -= Time.deltaTime;
            }
            else
            {
                _unfreezeDelay = unfreezeDelay;     //Reset the timer
                freeze = false;
                if (useDelayBetweenObjectUnfreeze)
                    StartCoroutine(FreezeChildrenDelayed(transform));
                else
                    FreezeChildren(transform);
                this.enabled = false;
            }
            return;
        }

        if (valuesChanged)
        {
            if (freezeAllChildren)
            {
                if (useDelayBetweenObjectUnfreeze && Application.isPlaying)
                    StartCoroutine(FreezeChildrenDelayed(transform));
                else
                    FreezeChildren(transform);
            }
            else
            {
                if (HasComponent<Rigidbody>(this.gameObject))
                {
                    transform.GetComponent<Rigidbody>().isKinematic = freeze;
                }
            }
        }
        valuesChanged = false;
    }
    IEnumerator FreezeChildrenDelayed(Transform Parent)
    {
        foreach (Transform child in Parent)
        {
            if (useDelayBetweenObjectUnfreeze)
            {
                yield return new WaitForSecondsRealtime(objectFreezeDelay);
            }
            if (HasComponent<Rigidbody>(child.gameObject))
            {
                child.GetComponent<Rigidbody>().isKinematic = freeze;
            }
            if(child.childCount >= minChildAmount)
            {
                StartCoroutine(FreezeChildrenDelayed(child));
            }
        }
    }
    void FreezeChildren(Transform Parent)
    {
        foreach (Transform child in Parent)
        {
            if (HasComponent<Rigidbody>(child.gameObject))
            {
                child.GetComponent<Rigidbody>().isKinematic = freeze;
            }
            if (child.childCount >= minChildAmount)
            {
                FreezeChildren(child);
            }
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}