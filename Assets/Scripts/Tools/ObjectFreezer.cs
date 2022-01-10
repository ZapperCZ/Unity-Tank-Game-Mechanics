using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectFreezer : MonoBehaviour
{
    [SerializeField] bool FreezeAllChildren = true;
    [SerializeField] bool Freeze = false;
    [SerializeField] bool UnfreezeOnPlayMode = true;
    [SerializeField] float unfreezeDelay = 1f;
    [SerializeField] int minChildAmount = 1;

    float _unfreezeDelay = float.MaxValue;
    bool valuesChanged = false;

    // Start is called before the first frame update
    private void Awake()
    {
        _unfreezeDelay = unfreezeDelay;
        /*
        if (UnfreezeOnPlayMode && Application.isPlaying)
        {
            Freeze = false;
        }
        */
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
                FreezeChildren(transform);
                this.enabled = false;
            }
            return;
        }

        if (valuesChanged)
        {
            if (FreezeAllChildren)
            {
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
    void FreezeChildren(Transform Parent)
    {
        foreach (Transform child in Parent)
        {
            if (HasComponent<Rigidbody>(child.gameObject))
            {
                child.GetComponent<Rigidbody>().isKinematic = Freeze;
            }
            if(child.childCount >= minChildAmount)
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