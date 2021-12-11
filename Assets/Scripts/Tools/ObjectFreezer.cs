using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectFreezer : MonoBehaviour
{
    [SerializeField] bool FreezeAllChildren = true;
    [SerializeField] bool UnfreezeOnPlayMode = true;
    [SerializeField] bool Freeze = false;

    bool valuesChanged = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (UnfreezeOnPlayMode && Application.isPlaying)
        {
            Freeze = false;
        }
        valuesChanged = true;
    }

    private void OnValidate()
    {
        valuesChanged = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (valuesChanged)
        {
            if (Application.isPlaying && !UnfreezeOnPlayMode)
                return;

            if (FreezeAllChildren)
            {
                FreezeChildren(this.transform);
            }
            else
            {
                if (HasComponent<Rigidbody>(this.gameObject))
                {
                    this.transform.GetComponent<Rigidbody>().isKinematic = Freeze;
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
            if(child.childCount > 1)
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
