using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetVehicle;
    public Vector3 offset;
    [HideInInspector] public Vector3 actualOffset;

    [SerializeField] float translateSpeed;
    [SerializeField] float rotationSpeed;

    void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
    }

    void HandleTranslation()
    {
        var targetPosition = targetVehicle.TransformPoint(actualOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = targetVehicle.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
