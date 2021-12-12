using UnityEngine;

[ExecuteInEditMode]
public class SprocketCollider : MonoBehaviour
{
    [Header("Sprocket Parameters")]
    [SerializeField] int toothAmount = 20;
    [SerializeField] float toothDiameter = 0.02f;
    [SerializeField] float sprocketDiameter = 1f;
    [SerializeField] bool dualSprocket = true;
    [SerializeField] float sprocketSpacing = 0.6f;              //Determines the spacing between the 2 sprockets
    [Header("Component Settings")]
    [SerializeField] bool usePhysicsMaterial = false;
    [SerializeField] PhysicMaterial sprocketPhysicMaterial;
    [Header("Debug")]
    [SerializeField] bool runOutsideEditMode = false;           //When enabled, the colliders cannot be altered by this script when play mode is enabled 
    [SerializeField] bool deleteColliders = false;              //A poor mans button for deleting colliders
    [SerializeField] string toothObjectName = "Sprocket Tooth";

    Transform Parent;
    bool regenerate = false;

    void Start()
    {
        if (Application.isPlaying && !runOutsideEditMode)
        {
            this.enabled = false;
            return;
        }
        Parent = transform;
    }
    void OnValidate()
    {
        if(toothAmount < 0)
        {
            toothAmount = 1;
        }
        regenerate = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (deleteColliders)
        {
            DestroySprocketCollider();
            deleteColliders = false;
        }
        if (regenerate)
        {
            GenerateSprocketCollider();
            regenerate = false;
        }
    }
    void GenerateSprocketCollider()
    {
        DestroySprocketCollider();
        for (int i = 0; i < toothAmount / 2; i++)
        {
            Debug.Log(i);
            GameObject tooth = new GameObject();
            tooth.layer = 7;                                             //Non-Static layer
            tooth.name = toothObjectName;
            tooth.AddComponent<CapsuleCollider>();
            if (usePhysicsMaterial)
            {
                tooth.GetComponent<CapsuleCollider>().material = sprocketPhysicMaterial;
            }
            tooth.transform.parent = Parent;
            tooth.transform.localPosition = new Vector3(0, 0, 0);
            float rotationZ = (float)i * 180 / (float)toothAmount * 2;

            tooth.transform.localRotation = Quaternion.Euler(90,0, rotationZ);
            tooth.transform.localScale = new Vector3(toothDiameter, sprocketDiameter, toothDiameter);    //Set it's size according to parameters
            Events.instance.Raise(new GameObjectCreated(tooth));
        }
    }
    void DestroySprocketCollider()
    {
        int childCount = Parent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform colliderToDelete = null;

            if (Application.isPlaying)
            {
                colliderToDelete = Parent.GetChild(i);
            }
            else if (Application.isEditor)
            {
                colliderToDelete = Parent.GetChild(0);      //The 0 element will be the parent link
            }
            if (colliderToDelete.name == toothObjectName)
            {
                DestroyGameObjectSafely(colliderToDelete.gameObject);
            }
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
}
