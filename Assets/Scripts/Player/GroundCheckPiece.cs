using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckPiece : MonoBehaviour
{
    public bool Grounded = false;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter - " + other.name);
        if (other.gameObject.layer == 0)     //Ground
        {
            Debug.Log("Grounded");
            Grounded = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit - " + other.name);
        if (other.gameObject.layer == 0)     //Ground
        {
            Debug.Log("Not Grounded");
            Grounded = false;
        }
    }
}
