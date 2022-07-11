using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Vector3 targetRotation;
    [SerializeField] GameObject targetParent;
    [SerializeField] float teleportDelay = 0;
    public bool activateTeleportation;

    [SerializeField] float internalTimer = 0;
    bool activateCountdown;

    void Start()
    {
        Activated();
    }
    void OnValidate()
    {
        if (activateTeleportation)
        {
            Activated();
        }
    }   
    void Update()
    {
        if (!activateCountdown)
            return;
        if (internalTimer < teleportDelay)
        {
            internalTimer += Time.deltaTime;
            return;
        }
        internalTimer = 0;
        activateCountdown = false;
        TeleportObject();
    }
    void Activated()
    {
        activateTeleportation = false;
        internalTimer = 0;
        activateCountdown = true;
    }
    void TeleportObject()
    {
        if(targetParent != null)
        {

            transform.parent = targetParent.transform;
            transform.localPosition = targetPosition;
            transform.localRotation = Quaternion.Euler(targetRotation);
            return;
        }
        transform.position = targetPosition;
        transform.rotation = Quaternion.Euler(targetRotation);
    }
}
