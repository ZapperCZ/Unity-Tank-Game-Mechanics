using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectFreezer : MonoBehaviour
{
    [SerializeField] bool FreezeAllChildren = true;
    [SerializeField] bool Freeze = false;
    [SerializeField] bool UnfreezeOnPlayMode = true;
    [SerializeField] float unfreezeDelay = 1f;
    [SerializeField] bool useDelayBetweenObjectUnfreeze = true;
    [SerializeField] float objectFreezeDelay = 0.05f;
    [SerializeField] int minChildAmount = 1;

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
    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying && UnfreezeOnPlayMode)
        {
            if (_unfreezeDelay >= 0)
            {
                _unfreezeDelay -= Time.deltaTime;
            }
            else
            {
                _unfreezeDelay = unfreezeDelay;
                Freeze = false;
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
            if (FreezeAllChildren)
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
                    transform.GetComponent<Rigidbody>().isKinematic = Freeze;
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
                child.GetComponent<Rigidbody>().isKinematic = Freeze;
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
                child.GetComponent<Rigidbody>().isKinematic = Freeze;
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