using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    float vertical, horizontal = 180;
    float scroll;
    [SerializeField] float sensitivity, maximumSpeed, minDistance, maxDistance, scrollSens;
    public float targetDistance;
    float currentDistance;
    [SerializeField] Vector2 MinMax;                      //The maximum and minimum angle
    public Transform FocusPoint;

    void Update()
    {
        GetInput();

        if(scroll < 0 && targetDistance < maxDistance)            //Scrolling away (down)
        {
            targetDistance += scrollSens;
        }
        else if (scroll > 0 && targetDistance > minDistance)      //Scrolling in (up)
        {
            targetDistance -= scrollSens;
        }

        CameraClippingAvoidance();

        vertical = Mathf.Clamp(vertical, MinMax.x, MinMax.y);

        transform.eulerAngles = new Vector3(vertical, horizontal + 180, 0);
        transform.position = FocusPoint.position - transform.forward * targetDistance;
    }
    void GetInput()
    {
        if(Time.timeScale == 0)
        {
            scroll = 0;
            return;
        }
        if (Input.GetButton("Arrow Look Modifier"))
        {
            vertical += Input.GetAxis("Arrow Look Vertical") * sensitivity * -1;
            horizontal += Input.GetAxis("Arrow Look Horizontal") * sensitivity;
        }
        else
        {
            vertical += Input.GetAxis("Mouse Y") * sensitivity * -1;
            horizontal += Input.GetAxis("Mouse X") * sensitivity;
        }

        scroll = Input.GetAxis("Mouse ScrollWheel");
    }
    void CameraClippingAvoidance()
    {
        Vector3 cameraDirection = (transform.position - FocusPoint.position).normalized;            //Direction from focus point to the camera

        RaycastHit[] hits = Physics.RaycastAll(FocusPoint.position, cameraDirection, targetDistance);

        List<RaycastHit> hitList = hits.OrderBy(x => Vector2.Distance(FocusPoint.position, x.point)).ToList();

        foreach (RaycastHit hit in hitList)
        {
            if (!hit.transform.CompareTag("Player Controlled"))
            {
                targetDistance = Vector3.Distance(FocusPoint.position, hit.point);
                return;
            }
        }
    }
    float normalizeNumber(float num, float max)
    {
        return Mathf.Abs(num / max);
    }
}
