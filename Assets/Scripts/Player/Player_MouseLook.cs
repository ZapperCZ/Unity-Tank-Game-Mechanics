using UnityEngine;

public class Player_MouseLook : MonoBehaviour
{
    [Range(10f, 1000f)]
    [SerializeField] float sensitivity = 100f;
    [SerializeField] Transform Player;
    float horizontal, vertical;

    float xRotation = 0f;


    void Start()
    {
        Debug.Log("Player MouseLook - Initialized");
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetButton("Arrow Look Modifier"))
        {
            vertical = Input.GetAxis("Arrow Look Vertical") * sensitivity * 0.05f;
            horizontal = Input.GetAxis("Arrow Look Horizontal") * sensitivity * 0.05f;
        }
        else
        {
            horizontal = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            vertical = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        }

        xRotation -= vertical;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Player.Rotate(Vector3.up * horizontal);
    }
}