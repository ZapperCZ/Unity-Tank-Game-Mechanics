using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    float x, y;
    public float sensitivity, distance;
    public Vector2 MinMax;
    public Transform FocusPoint;

    void Update()
    {
        x += Input.GetAxis("Mouse Y") * sensitivity * -1;
        y += Input.GetAxis("Mouse X") * sensitivity;

        x = Mathf.Clamp(x, MinMax.x, MinMax.y);

        transform.eulerAngles = new Vector3(x, y + 180, 0);

        transform.position = FocusPoint.position - transform.forward * distance;
    }
}
