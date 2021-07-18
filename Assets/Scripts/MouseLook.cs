using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float Sensitivity = 100f;
    public Transform Player;
    public Transform Camera;
    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

        Player.Rotate(Vector3.up * xRotation);
        //Camera.Rotate();
    }
}
