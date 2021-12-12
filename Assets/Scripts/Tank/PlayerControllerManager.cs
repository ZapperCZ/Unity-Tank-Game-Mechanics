using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : MonoBehaviour
{
    public Transform Player;

    public Camera PlayerCamera;
    public Camera TankCamera;
    public GameObject test;

    public CharacterController CharacterController;
    public Player_Movement PlayerMovementScript;

    public int vehicleEnterRange = 3;
    public bool isInTank = false;
    TankController TankControllerScript;
    Transform Tank;
    void Start()
    {
        Debug.LogWarning(test.transform.parent);
    }
    void Update()
    {
        RaycastHit raycastTarget;
        if (PlayerMovementScript.enabled && Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out raycastTarget, vehicleEnterRange))
        {
            GameObject absoluteParent = FindAbsoluteParentWithComponent<TankController>(raycastTarget.transform.gameObject);
            if (absoluteParent != null)
            {
                if (Input.GetButtonDown("Use Key"))
                {
                    Tank = absoluteParent.transform;
                    TankControllerScript = absoluteParent.GetComponent<TankController>();
                    SwitchController();
                }
            }
        }
    }
    public void SwitchController(bool isExiting = false, bool isSpaceOnLeft = true, bool isSpaceOnRight = true)
    {
        if (isExiting)
        {
            Player.SetParent(null);
            TankCamera.transform.SetParent(Player);
            if (isSpaceOnLeft)
            {
                Player.position = Tank.TransformPoint(Tank.GetComponent<TankController>().leftExitPos);
            }
            else if (isSpaceOnRight)
            {
                Player.position = Tank.TransformPoint(Tank.GetComponent<TankController>().rightExitPos);
            }
            else
            {
                Player.position = Tank.position + new Vector3(0, 2, 0);
            }
            Player.rotation = Quaternion.Euler(0, Tank.eulerAngles.y, 0);
            CharacterController.enabled = isExiting;
        }
        else
        {
            CharacterController.enabled = isExiting;
            Player.SetParent(Tank);
            Player.localPosition = new Vector3(0, 0, 0);
            TankCamera.transform.SetParent(null);
            TankCamera.GetComponent<CameraFollow>().targetVehicle = Tank;
            if (TankControllerScript.CameraOffset != new Vector3(0, 0, 0))
            {
                TankCamera.GetComponent<CameraFollow>().actualOffset = TankControllerScript.CameraOffset;
            }
            else
            {
                TankCamera.GetComponent<CameraFollow>().actualOffset = TankCamera.GetComponent<CameraFollow>().offset;
            }

            /*  Stop the tank
            TankControllerScript.LeftSprocket
            TankControllerScript.RightSprocket
            */
        }
        PlayerMovementScript.enabled = isExiting;
        PlayerCamera.gameObject.SetActive(isExiting);

        isInTank = !isExiting;
        TankCamera.gameObject.SetActive(!isExiting);
        TankControllerScript.enabled = !isExiting;
    }
    GameObject FindAbsoluteParentWithComponent<T>(GameObject inputObject) where T : Component
    {
        GameObject objectWithComponent;
        try
        {
            objectWithComponent = HasComponent<T>(inputObject) ? inputObject : FindAbsoluteParentWithComponent<T>(inputObject.transform.parent.gameObject);
        }
        catch
        {
            objectWithComponent = null;
        }
        return objectWithComponent;
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
