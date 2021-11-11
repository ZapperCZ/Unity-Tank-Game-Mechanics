using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbing : MonoBehaviour
{
    public bool isGrabbing = false;
    [SerializeField] float grabStrength = 1;
    [SerializeField] float weightLimit = 20;
    [SerializeField] float distanceFromCamera = 1.2f;
    [SerializeField] float grabbingDistance = 5;
    [SerializeField] float distanceLimit = 3;
    [SerializeField] float grabbedDrag = 10;
    [SerializeField] LayerMask grabbingMask;

    [SerializeField] Camera Camera;

    float originalDrag;
    GameObject CurrentlyGrabbedObject = null;

    void Update()
    {
        if (Input.GetButtonDown("Grab Object"))
        {
            if(CurrentlyGrabbedObject == null)      //Pick the item up
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, grabbingDistance, grabbingMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform.CompareTag("Grabbable") && hit.transform.GetComponent<Rigidbody>().mass < weightLimit)
                    {
                        CurrentlyGrabbedObject = hit.transform.gameObject;
                        originalDrag = CurrentlyGrabbedObject.GetComponent<Rigidbody>().drag;   //Save the original drag value
                        CurrentlyGrabbedObject.GetComponent<Rigidbody>().drag = grabbedDrag;    //increase drag to prevent the object flying around the grabPoint
                        isGrabbing = true;
                        //Debug.Log("Grabbed - " + CurrentlyGrabbedObject.name);
                    }
                }
            }
            else            //Drop the item
            {
                //Debug.Log("Dropped - " + CurrentlyGrabbedObject.name);
                CurrentlyGrabbedObject.GetComponent<Rigidbody>().drag = originalDrag;   //set the drag back to it's original value
                CurrentlyGrabbedObject = null;
                isGrabbing = false;
            }
        }
        if (Application.isEditor)               //Debug feature
        {
            if (CurrentlyGrabbedObject != null && Input.GetButtonUp("Right Click"))
            {
                //Freeze / Unfreeze the object
                CurrentlyGrabbedObject.GetComponent<Rigidbody>().isKinematic = !CurrentlyGrabbedObject.GetComponent<Rigidbody>().isKinematic;
            }
        }
    }

    void FixedUpdate()
    {
        if (CurrentlyGrabbedObject != null)
        {
            Vector3 desiredPosition = Camera.transform.forward * distanceFromCamera + Camera.transform.position;
            Vector3 currentPosition = CurrentlyGrabbedObject.transform.position;
            Vector3 direction;
            float distance = (desiredPosition - currentPosition).magnitude;

            if (distance > distanceLimit)
            {
                Debug.Log("Dropped - " + CurrentlyGrabbedObject.name);
                CurrentlyGrabbedObject.GetComponent<Rigidbody>().drag = originalDrag;
                CurrentlyGrabbedObject = null;
                isGrabbing = false;
                return;
            }

            if (distance > 1)    //Normalize it if it is bigger than 1
            {
                direction = (desiredPosition - currentPosition) / (desiredPosition - currentPosition).magnitude;
            }
            else
            {
                direction = (desiredPosition - currentPosition);
            }

            CurrentlyGrabbedObject.transform.GetComponent<Rigidbody>().AddForce(direction * grabStrength);
        }
    }
}
