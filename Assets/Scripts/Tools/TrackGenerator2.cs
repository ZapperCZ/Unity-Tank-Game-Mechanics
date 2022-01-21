using UnityEngine;

[ExecuteInEditMode]
public class TrackGenerator2 : MonoBehaviour
{
    enum TrackType
    {
        Basic,
        Complicated
    }

    [Header("Track Objects")]
    [SerializeField] GameObject TrackLink;
    [SerializeField] bool UseBracketLocalPosition = true;
    [SerializeField] GameObject LeftConnectingBracket;
    [SerializeField] GameObject MiddleConnectingBracket;
    [SerializeField] GameObject RightConnectingBracket;
    [SerializeField] Transform ConnectingBracketHingePosition;
    [Header("Track Settings")]
    [Range(1, 20)]
    [SerializeField] float trackLength = 10f;               //The length of a track
    [Range(0, 0.5f)]
    [SerializeField] float linkSpacing = 0.2f;              //Amount of space between the track links
    //[SerializeField] string parentName = "Track";           //The name of the track parent
    [Header("Joint Settings")]
    [Range(1, 180)]
    [SerializeField] float linkJointAngleLimit = 120;       //The maximum angle between the track links
    [SerializeField] bool useCustomBreakForce = false;
    [SerializeField] float jointBreakForce = 20000;
    [SerializeField] bool useCustomBreakTorque = false;
    [SerializeField] float jointBreakTorque = 10000;
    [Header("Script Settings")]
    [SerializeField] bool generateOnChange = false;
    [Header("Debug")]
    [SerializeField] bool runOutsideEditMode = false;           //When enabled, the tracks won't be generated by the script at runtime
    [SerializeField] bool generateTracks = false;
    [SerializeField] bool deleteTracks = false;

    bool createTracks = false;
    bool destroyTracks = false;

    //FIX: Script loses track of the track parent, disabling the generation of parent doesn't work once this happens

    void Update()
    {
        if (!runOutsideEditMode && Application.isPlaying)
        {
            this.enabled = false;
            return;
        }
        if (transform.hasChanged && !Application.isPlaying) //Only generate tracks on moving the track link when the game is in editor mode
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
        if (TrackLink == null || LeftConnectingBracket == null || MiddleConnectingBracket == null || RightConnectingBracket == null)
        {
            Debug.LogError("Track Builder - " + this.name + " - Required components not found, disabling script. Make sure that the script has all GameObjects assigned");
            this.enabled = false;
            return;
        }
        /*
        TrackLink.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0);
        TrackLink.GetComponent<Rigidbody>().inertiaTensor = new Vector3(1, 1, 1);
        */
    }
    void OnValidate()
    {
/*
        if (parentName == "")
        {
            parentName = "Track";
            return;
        }
*/
        if (generateTracks == true)
        {
            generateTracks = false;
            createTracks = true;
            return;
        }

        if (deleteTracks == true)
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
        //FIX: Tracks sometimes get generated twice when the script is unloaded and loaded (might do something with onLoad)
        DestroyTracks();
        Debug.Log($"Track Builder - {this.name} - Generating Tracks");
        /*        
                if (createParent)
                {
                    if (TrackLink.transform.parent.gameObject.name != parentName)        //Parent doesn't exist yet
                    {
                        TrackParent = new GameObject();
                        TrackParent.name = parentName;
                        TrackParent.transform.parent = TrackLink.transform.parent;
                        TrackLink.transform.parent = TrackParent.transform;
                    }
                }
                else
                {
                    if (TrackLink.transform.parent.gameObject.name == parentName)
                    {
                        TrackLink.transform.parent = TrackParent.transform.parent;
                        DestroyGameObjectSafely(TrackParent);
                    }
                }
        */
        Vector3 linkDimensions = BoundsSizeToVector3(TrackLink.GetComponent<MeshFilter>().sharedMesh.bounds);
        float linkLength;                   //Length of a link in the direction of the track
        float linkAmount;                   //Amount of links in one track
        Vector3 direction;                  //The direction of the track
        int trackDirection = 0;             //Track direction in relation to the main link axis, 0 - X, 1 - Z
        if (linkDimensions.x * TrackLink.transform.lossyScale.x > linkDimensions.z * TrackLink.transform.lossyScale.z)     //x is track width, z is track direction
        {
            linkLength = linkDimensions.z * TrackLink.transform.lossyScale.z;
            direction = TrackLink.transform.forward;
            trackDirection = 1;
        }
        else                                                                        //z is track width, x is track direction
        {
            linkLength = linkDimensions.x * TrackLink.transform.lossyScale.x;
            direction = Quaternion.Euler(0, 90, 0) * TrackLink.transform.forward;   //Rotates the Z direction by 90 degrees on Y axis to achieve X direction
        }

        Debug.Log(linkLength);

        linkAmount = Mathf.Round(trackLength / (linkLength + linkSpacing));
        GameObject previousTrackLink = TrackLink;
        for (int i = 1; i < linkAmount; i++)
        {
            GameObject newTrackLink = Instantiate(TrackLink);               //Create a new link
            newTrackLink.transform.parent = this.transform;                 //Set it to have the same parent

            Vector3 offset;

/*
            if (trackDirection == 0)                                         //The track is heading on the x axis of the parent
            {
                offset = direction * i * (TrackLink.transform.lossyScale.x + linkSpacing);  //Offset of the new track link from the previous one
            }
            else                                                            //The track is heading on the z axis of the parent
            {
                offset = direction * i * (TrackLink.transform.lossyScale.z + linkSpacing);  //Offset of the new track link from the previous one
            }
*/
            offset = direction * i * (linkLength + linkSpacing);

            newTrackLink.transform.localPosition = offset;  //Apply offset

            newTrackLink.name = TrackLink.name + " " + i.ToString();

            newTrackLink.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0);
            newTrackLink.GetComponent<Rigidbody>().inertiaTensor = new Vector3(1, 1, 1);        //Set these 2 values because otherwise physics just decide to jump out the window, don't ask me why
/*
            if (useCustomBreakForce)
            {
                newTrackLink.GetComponent<HingeJoint>().breakForce = jointBreakForce;
            }
            if (useCustomBreakTorque)
            {
                newTrackLink.GetComponent<HingeJoint>().breakTorque = jointBreakTorque;
            }
            Vector3 hingeAxis;
            Vector3 hingeAnchor;
            float ratio;
            float anchOffset;
            if (trackDirection == 1)
            {
                hingeAxis = new Vector3(1, 0, 0);
                ratio = TrackLink.transform.lossyScale.z / linkSpacing;
                anchOffset = 0.5f + (0.5f / ratio);
                hingeAnchor = new Vector3(0, 0, -anchOffset);
            }
            else
            {
                hingeAxis = new Vector3(0, 0, 1);
                ratio = TrackLink.transform.lossyScale.x / linkSpacing;
                anchOffset = 0.5f + (0.5f / ratio);
                hingeAnchor = new Vector3(-anchOffset, 0, 0);
            }
            newTrackLink.GetComponent<HingeJoint>().axis = hingeAxis;
            newTrackLink.GetComponent<HingeJoint>().anchor = hingeAnchor;
*/

            Events.instance.Raise(new GameObjectCreated(newTrackLink));
            previousTrackLink = newTrackLink;
        }
    }
    void DestroyTracks()
    {
        Debug.Log("Track Builder - Destroying Tracks");
        Transform Parent = transform;
        int childCount = Parent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform linkToDelete = null;
            if (Application.isPlaying)
            {
                linkToDelete = Parent.GetChild(i);
            }
            else if (Application.isEditor)
            {
                linkToDelete = Parent.GetChild(0);      //The 0 element will be the parent link
            }
            DestroyGameObjectSafely(linkToDelete.gameObject);
        }
    }
    void DestroyObjectSafely(Object obj)
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
    void DestroyGameObjectSafely(GameObject obj)            //Destroys the object appropriately to the current mode
    {
        if (Application.isPlaying)
        {
            if (HasComponent<Collider>(obj))
            {
                //Sends an event to DevCollider where the object is removed from lists and then destroyed
                Events.instance.Raise(new GameObjectDeleted(obj));
            }
            else
            {
                Destroy(obj);
            }
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
    Vector3 BoundsSizeToVector3(Bounds inputBounds)
    {
        Vector3 result = new Vector3(inputBounds.size.x,inputBounds.size.y,inputBounds.size.z);
        return result;
    }
}
