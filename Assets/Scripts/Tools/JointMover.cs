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
    [SerializeField] Transform FinalLocation;
    [SerializeField] float movementSpeed = 0.05f;
    private Rigidbody ConnectedBody;

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
                }
                break;
            default:
                Debug.LogError(this.GetType().Name + " - " + this.name + " - Joint type not yet implemented");
                this.enabled = false;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
