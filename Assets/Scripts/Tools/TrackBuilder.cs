using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackBuilder : MonoBehaviour
{
    [SerializeField]
    enum TrackType
    {
        Flat,
        Loop
    }
    [Range(5,100)]
    [SerializeField] float trackLength = 10f;
    [Range(0.05f,1f)]
    [SerializeField] float linkSpacing;

    GameObject TrackLink;

    private void OnEnable()
    {
        TrackLink = this.gameObject;    //Uses the object it is assigned to as a track link
        if(!HasComponent<MeshRenderer>(TrackLink) || !HasComponent<Collider>(TrackLink) || !HasComponent<Rigidbody>(TrackLink))
        {
            Debug.LogError("Track Builder - " + TrackLink.name + " - Required components not found, disabling script. Make sure that the object has a MeshRenderer, a Collider and a Rigidbody");
            this.enabled = false;
        }
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a collider or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
