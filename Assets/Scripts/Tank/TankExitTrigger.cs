using UnityEngine;

public class TankExitTrigger : MonoBehaviour
{
    [SerializeField] TankController TankController;

    void Start()
    {
        TankController = this.GetComponentInParent<TankController>();
        if (transform.localPosition.x < 0)
        {
            TankController.leftExitPos = new Vector3(transform.localPosition.x, 0, 0);
        }
        else
        {
            TankController.rightExitPos = new Vector3(transform.localPosition.x, 0, 0);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            TankController.EvaluateTrigger(transform.localPosition.x, false);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            TankController.EvaluateTrigger(transform.localPosition.x, true);
        }
    }
}
