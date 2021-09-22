using UnityEngine;

public class Player_MouseLook : MonoBehaviour
{
    [Range(10f, 1000f)]
    [SerializeField] float Sensitivity = 100f;
    [SerializeField] Transform Player;

    float xRotation = 0f;


    void Start()
    {
        Debug.Log("Player MouseLook - Initialized");
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Player.Rotate(Vector3.up * mouseX);
    }
}