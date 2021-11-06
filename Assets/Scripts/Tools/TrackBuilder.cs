using UnityEngine;

[ExecuteInEditMode]
public class TrackBuilder : MonoBehaviour
{
    [Range(1,20)]
    [SerializeField] float trackLength = 10f;               //The length of a track
    [Range(0,0.5f)]
    [SerializeField] float linkSpacing = 0.2f;              //Amount of space between the track links
    [Range(1,180)]
    [SerializeField] float linkJointAngleLimit = 120;       //The maximum angle between the track links
    [SerializeField] bool generateTracks = false;
    [SerializeField] string parentName = "Track";           //The name of the track parent
    [SerializeField] bool generateOnChange = true;
    [SerializeField] bool deleteTracks = false;

    GameObject TrackLink;
    GameObject TrackParent;
    bool createParent = true;                               //Whether a parent for the track links should be created or not
    bool createTracks = false;
    bool destroyTracks = false;

    //FIX: Script loses track of the track parent, disabling the generation of parent doesn't work once this happens

    void Update()
    {
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
        TrackLink = this.gameObject;    //Uses the object it is assigned to as a track link
        if(!HasComponent<MeshRenderer>(TrackLink) || !HasComponent<Collider>(TrackLink) || !HasComponent<Rigidbody>(TrackLink))
        {
            Debug.LogError("Track Builder - " + TrackLink.name + " - Required components not found, disabling script. Make sure that the object has a MeshRenderer, a Collider and a Rigidbody");
            this.enabled = false;
            return;
        }
        TrackLink.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0);
        TrackLink.GetComponent<Rigidbody>().inertiaTensor = new Vector3(1, 1, 1);
    }
    void OnValidate()
    {
        if(parentName == "")
        {
            parentName = "Track";
            return;
        }

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
        //FIX: Tracks sometimes get generated twice when the script is unloaded and loaded (maybe change the deletion method)
        DestroyTracks();
        Debug.Log("Track Builder - Generating Tracks");
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
        float linkLength;                   //Length of a link in the direction of the track
        float linkAmount;                   //Amount of links in one track
        Vector3 direction;                  //The direction of the track
        int trackDirection = 0;             //Track direction in relation to the main link axis, 0 - X, 1 - Z
        if(TrackLink.transform.lossyScale.x > TrackLink.transform.lossyScale.z)     //x is track width, z is track direction
        {
            linkLength = TrackLink.transform.lossyScale.z;
            direction = TrackLink.transform.forward;
            trackDirection = 1;
        }
        else                                                                        //z is track width, x is track direction
        {
            linkLength = TrackLink.transform.lossyScale.x;
            direction = Quaternion.Euler(0 ,90 ,0) * TrackLink.transform.forward;   //Rotates the Z direction by 90 degrees on Y axis to achieve X direction
        }

        linkAmount = Mathf.Round(trackLength/(linkLength+linkSpacing)) - 1;         //Decrease by one to account for the already existing parent link
        GameObject previousTrackLink = TrackLink;
        for(int i = 1; i <  linkAmount; i++)
        {
            GameObject newTrackLink = Instantiate(TrackLink);
            newTrackLink.transform.parent = TrackLink.transform.parent;
            Vector3 offset;
            if(trackDirection == 0)
            {
                offset = direction * i * (TrackLink.transform.lossyScale.x + linkSpacing);
            }
            else
            {
                offset = direction * i * (TrackLink.transform.lossyScale.z + linkSpacing);
            }
            newTrackLink.transform.localPosition = TrackLink.transform.localPosition + offset;
            newTrackLink.name = TrackLink.name + " " + i.ToString();
            newTrackLink.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, 0, 0);
            newTrackLink.GetComponent<Rigidbody>().inertiaTensor = new Vector3(1, 1, 1);
            newTrackLink.AddComponent<HingeJoint>().connectedBody = previousTrackLink.GetComponent<Rigidbody>();
            Vector3 hingeAxis;
            Vector3 hingeAnchor;
            float ratio;
            float anchOffset;
            if(trackDirection == 1)
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
            DestroyObjectSafely(newTrackLink.GetComponent<TrackBuilder>());
            Events.instance.Raise(new GameObjectCreated(newTrackLink));
            previousTrackLink = newTrackLink;
        }
    }
    void DestroyTracks()
    {
        Debug.Log("Track Builder - Destroying Tracks");
        Transform Parent = transform.parent;
        int childCount = Parent.childCount-1;           //The parent can't be deleted

        for (int i = 1; i <= childCount; i++)
        {
            Transform linkToDelete = null;
            if (Application.isPlaying)
            {
                linkToDelete = Parent.GetChild(i);
            }
            else if (Application.isEditor)
            {
                linkToDelete = Parent.GetChild(1);      //The 0 element will be the parent link
            }
            if (linkToDelete.name.Contains(this.name) && !HasComponent<TrackBuilder>(linkToDelete.gameObject))
            {
                DestroyObjectSafely(linkToDelete.gameObject);
            }
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
    void DestroyObjectSafely(GameObject obj)            //Destroys the object appropriately to the current mode
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
}
