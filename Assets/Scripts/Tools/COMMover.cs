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
        currCOMPos = COM.localPosition;
        if (!HasComponent<Rigidbody>(this.gameObject))
        {
            Debug.LogError("COM Mover - " + this.name + "Required components not found, disabling script. Make sure that the parent object has a Rigidbody");
            this.enabled = false;
        }
    }
    void Update()
    {
        if(Application.isEditor && ((Application.isPlaying && RunInPlayMode) || !Application.isPlaying))
        {
            if(currCOMPos != COM.localPosition)
            {
                currCOMPos = COM.localPosition;
                if (ChangeChildrenCOM)
                {
                    ChangeCOMInChildren(this.transform);
                }
                else
                {
                    this.transform.GetComponent<Rigidbody>().centerOfMass = currCOMPos;
                }
                Debug.Log($"COM Mover - {this.transform.GetComponent<Rigidbody>().centerOfMass}");
            }
        }
    }
    void ChangeCOMInChildren(Transform Parent)
    {
        foreach (Transform child in Parent)
        {
            if (HasComponent<Rigidbody>(child.gameObject))
            {
                child.GetComponent<Rigidbody>().centerOfMass = currCOMPos;
            }
            if (child.childCount > 1)
            {
                ChangeCOMInChildren(child);
            }
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
