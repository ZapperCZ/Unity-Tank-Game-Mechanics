using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class JointMover : MonoBehaviour
{
    private enum JointType
    {
        FixedJoint,
    }

    [SerializeField] JointType TypeOfJoint = new JointType();
    [SerializeField] Transform TargetPosition;
    [SerializeField] float movementSpeed = 0.01f;

    private Rigidbody ConnectedBody;
    private float distanceToLocation;
    private bool originalRigidbodyState;
    private Vector3 ChangeVector;
    private Vector3 LocationDirection;

    void Awake()
    {
        ConnectedBody = transform.GetComponent<Joint>().connectedBody;
        originalRigidbodyState = ConnectedBody.isKinematic;
    }
    void Start()
    {
        transform.GetComponent<Rigidbody>().isKinematic = true;

        switch (TypeOfJoint)
        {
            case 0:
                if (HasComponent<FixedJoint>(transform.gameObject))
                {
                    ConnectedBody = transform.GetComponent<FixedJoint>().connectedBody;
                    Destroy(transform.GetComponent<FixedJoint>());
                }
                else
                {
                    Debug.LogError(this.GetType().Name + " - " + this.name + " - Selected joint not found on GameObject");
                    this.enabled = false;
                    return;
                }
                break;
            default:
                Debug.LogError(this.GetType().Name + " - " + this.name + " - Joint type not yet implemented");
                this.enabled = false;
                return;
        }
        ConnectedBody.isKinematic = true;
    }
    void FixedUpdate()
    {
        LocationDirection = (TargetPosition.position - transform.position).normalized;
        distanceToLocation = Vector3.Distance(transform.position, TargetPosition.position);
        ChangeVector = LocationDirection * movementSpeed;

        if (distanceToLocation > 0.02f)
        {
            transform.position += ChangeVector;
            return;
        }

        //This part only runs when the object is in it's new position
        switch (TypeOfJoint)
        {
            case 0:
                transform.gameObject.AddComponent<FixedJoint>().connectedBody = ConnectedBody;
                break;
        }

        transform.GetComponent<Rigidbody>().isKinematic = false;
        ConnectedBody.isKinematic = originalRigidbodyState;
        this.enabled = false;
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
