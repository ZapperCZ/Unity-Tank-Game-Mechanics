using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbing : MonoBehaviour
{
    [SerializeField] float grabStrength = 1;
    [SerializeField] float weightLimit = 20;
    [SerializeField] float distanceFromCamera = 1.2f;
    [SerializeField] float grabbingDistance = 5;
    [SerializeField] LayerMask grabbingMask;

    [SerializeField] Camera Camera;

    GameObject CurrentlyGrabbedObject = null;
    //[SerializeField] - grabbableTags;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Grab Object"))
        {
            if(CurrentlyGrabbedObject == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, grabbingDistance, grabbingMask))
                {
                    Debug.Log("Grabbing");
                    if (hit.transform.tag == "Grabbable")
                    {
                        CurrentlyGrabbedObject = hit.transform.gameObject;
                        Debug.Log("Grabbed - " + CurrentlyGrabbedObject.name);
                    }
                }
            }
            else
            {
                Debug.Log("Dropped - " + CurrentlyGrabbedObject.name);
                CurrentlyGrabbedObject = null;
            }
        }
    }
}
