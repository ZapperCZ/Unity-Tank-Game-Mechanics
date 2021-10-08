using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackBuilder : MonoBehaviour
{
    /*
    enum TrackType
    {
        Flat,
        Loop
    }
    */
    //[SerializeField] TrackType TypeOfTrack = new TrackType();
    [Range(1,20)]
    [SerializeField] float trackLength = 10f;               //The length of a track
    [Range(0,0.5f)]
    [SerializeField] float linkSpacing = 0.2f;              //Amount of space between the track links
    [Range(1,180)]
    [SerializeField] float linkJointAngleLimit = 120;       //The maximum angle between the track links
    [SerializeField] bool generateTracks = false;
    [SerializeField] bool createParent = true;              //Whether a parent for the track links should be created or not
    [SerializeField] string parentName = "Track";           //The name of the track parent
    [SerializeField] bool generateOnChange = true;
    [SerializeField] bool deleteTracks = false;

    List<GameObject> TrackLinks = new List<GameObject>();
    GameObject TrackLink;
    GameObject TrackParent;
    bool createTracks = false;
    bool destroyTracks = false;

    void Update()
    {
        if (transform.hasChanged)
        {
            createTracks = true;
            transform.hasChanged = false;
        }
        if (createTracks)
        {
            createTracks = false;
            GenerateTracks();
        }
        if (destroyTracks)
        {
            destroyTracks = false;
            DestroyTracks();
        }
    }
    void OnEnable()
    {
        TrackLink = this.gameObject;    //Uses the object it is assigned to as a track link
        if(!HasComponent<MeshRenderer>(TrackLink) || !HasComponent<Collider>(TrackLink) || !HasComponent<Rigidbody>(TrackLink))
        {
            Debug.LogError("Track Builder - " + TrackLink.name + " - Required components not found, disabling script. Make sure that the object has a MeshRenderer, a Collider and a Rigidbody");
            this.enabled = false;
            return;
        }
    }
    void OnDisable()
    {
        DestroyTracks();
    }
    void OnValidate()
    {
        if (generateTracks == true)
        {
            generateTracks = false;
            createTracks = true;
            return;
        }

        if(deleteTracks == true) 
        {
            deleteTracks = false;
            destroyTracks = true;
            return;
        }

        if (generateOnChange)
        {
            createTracks = true;
        }
    }
    void GenerateTracks()
    {
        //FIX: Tracks get generated twice when the game is in play mode
        DestroyTracks();
        if (createParent)
        {
            if(TrackLink.transform.parent.gameObject.name != parentName)        //Parent doesn't exist yet
            {
                TrackParent = new GameObject();
                TrackParent.name = parentName;
                TrackParent.transform.parent = TrackLink.transform.parent;
                TrackLink.transform.parent = TrackParent.transform;
            }
        }
        else
        {
            if(TrackLink.transform.parent.gameObject.name == parentName)
            {
                TrackLink.transform.parent = TrackParent.transform.parent;

                DestroyObjectSafely(TrackParent);
            }
        }
        float linkLength;
        float linkAmount;
        Vector3 direction;
        int trackDirection = 0;             //0 - X, 1 - Z
        if(TrackLink.transform.lossyScale.x > TrackLink.transform.lossyScale.z)     //x is track width, z is track direction
        {
            linkLength = TrackLink.transform.lossyScale.z;
            direction = TrackLink.transform.forward;
            trackDirection = 1;
        }
        else                                                                        //z is track width, x is track direction
        {
            linkLength = TrackLink.transform.lossyScale.x;
            direction = Quaternion.Euler(0 ,90 ,0) * TrackLink.transform.forward;  //Rotates the Z direction by 90 degrees to achieve X direction
        }

        linkAmount = Mathf.Round(trackLength/(linkLength+linkSpacing));
        for(int i = 1; i <  linkAmount; i++)
        {
            GameObject newTrackLink = Instantiate(TrackLink);
            Vector3 offset;
            if(trackDirection == 0)
            {
                offset = direction * i * (TrackLink.transform.lossyScale.x + linkSpacing);
            }
            else
            {
                offset = direction * i * (TrackLink.transform.lossyScale.z + linkSpacing);
            }
            newTrackLink.transform.position += offset;
            newTrackLink.name = TrackLink.name + " " + i.ToString();
            newTrackLink.transform.parent = TrackLink.transform.parent;
            DestroyObjectSafely(newTrackLink.GetComponent<TrackBuilder>());
            
            TrackLinks.Add(newTrackLink);
        }
    }
    void DestroyTracks()
    {
        for(int i = 0; i < TrackLinks.Count; i++)
        {
            DestroyObjectSafely(TrackLinks[i]);
        }
        TrackLinks = new List<GameObject>();
    }
    void DestroyObjectSafely(Object obj)            //Destroys the object appropriately to the current mode
    {
        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else if (Application.isEditor)
        {
            DestroyImmediate(obj);
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
