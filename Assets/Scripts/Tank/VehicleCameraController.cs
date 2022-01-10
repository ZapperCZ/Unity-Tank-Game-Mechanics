using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    float x, y = 180;
    float scroll;
    public float sensitivity, distance, minDistance, maxDistance, scrollSens;
    public Vector2 MinMax;                      //The maximum and minimum angle
    public Transform FocusPoint;

    void Update()
    {
        GetInput();

        if(scroll < 0 && distance < maxDistance)            //Scrolling away (down)
        {
            distance += scrollSens;
        }
        else if (scroll > 0 && distance > minDistance)      //Scrolling in (up)
        {
            distance -= scrollSens;
        }

        x = Mathf.Clamp(x, MinMax.x, MinMax.y);

        transform.eulerAngles = new Vector3(x, y + 180, 0);
        transform.position = FocusPoint.position - transform.forward * distance;
    }
    void GetInput()
    {
        x += Input.GetAxis("Mouse Y") * sensitivity * -1;
        y += Input.GetAxis("Mouse X") * sensitivity;

        scroll = Input.GetAxis("Mouse ScrollWheel");
    }
}
