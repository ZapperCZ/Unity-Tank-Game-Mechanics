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

    void Update()
    {
        if (Input.GetButtonDown("Grab Object"))
        {
            if(CurrentlyGrabbedObject == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, grabbingDistance, grabbingMask))
                {
                    if (hit.transform.tag == "Grabbable")
                    {
                        CurrentlyGrabbedObject = hit.transform.gameObject;
                        Debug.Log("Grabbed - " + CurrentlyGrabbedObject.name);
                        CurrentlyGrabbedObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                }
            }
            else
            {
                Debug.Log("Dropped - " + CurrentlyGrabbedObject.name);
                CurrentlyGrabbedObject.GetComponent<Rigidbody>().useGravity = true;
                CurrentlyGrabbedObject = null;
            }
        }
    }

    void FixedUpdate()
    {
        if(CurrentlyGrabbedObject != null)
        {
            CurrentlyGrabbedObject.transform.position = Camera.transform.forward * distanceFromCamera + Camera.transform.position;
        }
    }
}
