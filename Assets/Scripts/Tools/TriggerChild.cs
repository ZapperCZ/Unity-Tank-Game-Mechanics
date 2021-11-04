using UnityEngine;

public class TriggerChild : MonoBehaviour
{
    public bool isTriggered = false;
    public LayerMask CollisionMask;

    Transform Parent;

    private void Start()
    {
        Parent = this.transform.parent;
    }
    private void Update()
    {
        CollisionMask = Parent.GetComponent<TriggerChildManager>().CollisionMask;   //TODO: Find a more efficient way to do this
    }
    public void OnTriggerEnter(Collider other)
    {
        if (CollisionMask == (CollisionMask | (1<<other.gameObject.layer)))         //Collided object has a layer contained in the Collision LayerMask
        {
            isTriggered = true;
            this.transform.parent.GetComponent<TriggerChildManager>().isTriggered = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (CollisionMask == (CollisionMask | (1 << other.gameObject.layer)))
        {
            bool canChange = true;
            isTriggered = false;
            foreach(Transform childTrigger in Parent)
            {
                if (childTrigger.GetComponent<TriggerChild>())
                {
                    if (childTrigger.GetComponent<TriggerChild>().isTriggered)
                    {
                        canChange = false;
                        break;
                    }
                }
            }
            if (canChange)
            {
                Parent.GetComponent<TriggerChildManager>().isTriggered = false;
            }
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a collider or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}