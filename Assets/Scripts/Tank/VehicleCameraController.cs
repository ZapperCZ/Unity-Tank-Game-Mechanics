using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    float x, y = 180;
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

        x = Mathf.Clamp(x, MinMax.x, MinMax.y);

        transform.eulerAngles = new Vector3(x, y + 180, 0);
        transform.position = FocusPoint.position - transform.forward * targetDistance;
    }
    void GetInput()
    {
        x += Input.GetAxis("Mouse Y") * sensitivity * -1;
        y += Input.GetAxis("Mouse X") * sensitivity;

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

        /*
        RaycastHit hit;
        if (Physics.Raycast(FocusPoint.position, cameraDirection, out hit, maxDistance))
        {
            targetDistance = Vector3.Distance(FocusPoint.position, hit.point);
        }
        */
    }
    float normalizeNumber(float num, float max)
    {
        return Mathf.Abs(num / max);
    }
}
