using UnityEngine;

public class TriggerChild : MonoBehaviour
{
    public bool isTriggered = false;

    Transform Parent;

    private void Start()
    {
        Parent = this.transform.parent;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0)     //Ground
        {
            isTriggered = true;
            this.transform.parent.GetComponent<TriggerChildManager>().isTriggered = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 0)     //Ground
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
