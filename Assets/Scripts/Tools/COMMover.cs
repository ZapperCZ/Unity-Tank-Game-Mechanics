using UnityEngine;

[ExecuteInEditMode]
public class COMMover : MonoBehaviour
{
    [SerializeField] bool RunInPlayMode = false;
    [SerializeField] bool ChangeChildrenCOM = false;
    [SerializeField] Transform COM;
    Vector3 currCOMPos;

    void Awake()
    {
        if (!HasComponent<Rigidbody>(this.gameObject))
        {
            Debug.LogError("COM Mover - " + this.name + "Required components not found, disabling script. Make sure that the parent object has a Rigidbody");
            this.enabled = false;
        }
        currCOMPos = COM.position;
        UpdateCOM();
    }
    void Update()
    {
        if (currCOMPos != COM.position)
        {
            currCOMPos = COM.position;
            UpdateCOM();
            Debug.Log($"COM Mover - {this.transform.GetComponent<Rigidbody>().centerOfMass}");
        }

        if (Application.isPlaying && (!RunInPlayMode || !Application.isEditor))
        {
            Debug.Log($"COM Mover - {this.transform.GetComponent<Rigidbody>().centerOfMass}");
            this.enabled = false;
        }
    }
    void UpdateCOM()
    {
        this.transform.GetComponent<Rigidbody>().centerOfMass = currCOMPos;
        if (ChangeChildrenCOM)
        {
            ChangeCOMInChildren(this.transform, currCOMPos);
        }
    }
    void ChangeCOMInChildren(Transform Parent, Vector3 COMPos)
    {
        foreach (Transform child in Parent)
        {
            if (HasComponent<Rigidbody>(child.gameObject))
            {
                child.GetComponent<Rigidbody>().centerOfMass = currCOMPos;
            }
            if (child.childCount > 1)
            {
                ChangeCOMInChildren(child, COMPos);
            }
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
