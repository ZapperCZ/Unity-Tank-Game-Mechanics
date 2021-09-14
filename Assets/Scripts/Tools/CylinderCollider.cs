using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CylinderCollider : MonoBehaviour
{
    [Range(6, 50)]
    [SerializeField] int cylinderSides = 16;
    [Range(0.1f, 10)]
    [SerializeField] float cylinderSideWidth = 10;
    [SerializeField] bool sideWidthLocked = true;
    [Range(0.1f, 5)]
    [SerializeField] float cylinderDiameter = 1;
    [SerializeField] bool diameterLocked = true;
    [SerializeField] float cylinderHeight = 1;
    [SerializeField] bool isTrigger = false;
    [SerializeField] bool deleteColliders = false;

    Transform Parent;
    bool prevSideWidthLocked;
    bool prevDiameterLocked;
    //float prevSides;
    float prevSideWidth;
    float prevDiameter;
    bool regenerate = false;
    bool destroy = false;
    bool prevDeleteColliders;

    void Start()
    {
        prevDeleteColliders = deleteColliders;
        Parent = this.transform;
        prevSideWidth = cylinderSideWidth;
        prevDiameter = cylinderDiameter;
        prevSideWidthLocked = sideWidthLocked;
        prevDiameterLocked = diameterLocked;
        CreateCylinderCollider();
        Debug.Log($"Cylinder Collider - {Parent.name} - Initialized");
    }
    void OnValidate()
    {
        //Only even numbers
        if (cylinderSides % 2 == 1)
        {
            cylinderSides -= 1;
        }

        //Locks the currently changing variable
        //Has to be an else if otherwise they would affect each other
        if (prevSideWidth != cylinderSideWidth)
        {
            prevSideWidth = cylinderSideWidth;
            sideWidthLocked = true;
        }
        else if (prevDiameter != cylinderDiameter)
        {
            prevDiameter = cylinderDiameter;
            diameterLocked = true;
        }

        if(prevDeleteColliders != deleteColliders)
        {
            prevDeleteColliders = deleteColliders = false;
            destroy = true;
        }
        else
        {
            //Only 1 can be checked at the same time, but both can be unchecked
            if (sideWidthLocked && diameterLocked)
            {
                if (sideWidthLocked != prevSideWidthLocked)
                {
                    prevSideWidthLocked = sideWidthLocked;
                    diameterLocked = prevDiameterLocked = !sideWidthLocked;
                }
                if (diameterLocked != prevDiameterLocked)
                {
                    prevDiameterLocked = diameterLocked;
                    sideWidthLocked = prevSideWidthLocked = !diameterLocked;
                }
            }

            //TODO: Maybe switch the complex equation for my approximation once the n-gon gets complex

            if (sideWidthLocked)
            {
                //prevDiameter = cylinderDiameter = cylinderSides * cylinderSideWidth / Mathf.PI;               //My approximation
                prevDiameter = cylinderDiameter = cylinderSideWidth / Mathf.Tan(Mathf.PI / cylinderSides);      //Precise equation
            }
            if (diameterLocked)
            {
                //prevSideWidth = cylinderSideWidth = Mathf.PI * cylinderDiameter / cylinderSides;              //My approximation
                prevSideWidth = cylinderSideWidth = cylinderDiameter * Mathf.Tan(Mathf.PI / cylinderSides);     //Precise equation
            }
            regenerate = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (destroy)
        {
            DestroyCylinderCollider();
            destroy = false;
        }
        if (regenerate)
        {
            Debug.Log($"Cylinder Collider - {Parent.name} - Regenerating");
            regenerate = false;
            //TODO: Don't re-create the colliders, but instead change their values since that is less resource intensive
            CreateCylinderCollider();
        }
    }
    void DestroyCylinderCollider()
    {
        int childCount = Parent.childCount;

        for(int i = 0; i < childCount; i++)
        {
            Transform colliderToDelete = null;
            if (Application.isPlaying)
            {
                colliderToDelete = Parent.GetChild(i);
            }
            else if (Application.isEditor)
            {
                colliderToDelete = Parent.GetChild(0);
            }
            if (colliderToDelete.name == "CyllinderColliderPart")
            {
                if (Application.isPlaying)
                {
                    Destroy(colliderToDelete.gameObject);
                }
                else if (Application.isEditor)
                {
                    DestroyImmediate(colliderToDelete.gameObject);
                }
            }
        }
        
    }
    void CreateCylinderCollider()
    {
        DestroyCylinderCollider();
        Debug.Log($"Cyllinder Collider - {Parent.name} - Creating collider...");
        for (int i = 0; i < cylinderSides / 2; i++)
        {
            GameObject collider = new GameObject();
            collider.layer = 7;                                             //Non-Static layer
            collider.name = "CyllinderColliderPart";
            collider.AddComponent<BoxCollider>().isTrigger = isTrigger;
            collider.AddComponent<Rigidbody>().useGravity = false;          //Required for trigger to work properly
            collider.AddComponent<GroundCheckPiece>();                      //Script that handles collision detection
            collider.transform.SetParent(Parent);
            collider.transform.localPosition = new Vector3(0, 0, 0);        //Set it at the position of it's parent
            float rotationY = (float)i * 180 / (float)cylinderSides * 2;

            //TODO: Set the variables to be global and not local
            collider.transform.localRotation = Quaternion.Euler(0, rotationY, 0);   //Rotate it so the final shape forms an n-gon
            collider.transform.localScale = new Vector3(cylinderSideWidth, cylinderHeight, cylinderDiameter);    //Set it's size according to parameters
        }
        Debug.Log($"Cyllinder Collider - {Parent.name} - Done");
    }
}
