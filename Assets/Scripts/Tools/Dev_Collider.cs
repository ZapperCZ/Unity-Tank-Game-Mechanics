using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dev_Collider : MonoBehaviour
{
    [SerializeField] bool devMode = true;               //Controls whether the script initializes or not
    [SerializeField] bool isActive = false;             //The state of the collider view

    [SerializeField] GameObject[] IgnoredParents;       //Array of parent objects that will be ignored when creating collider models
    [SerializeField] Material[] Materials;              //Array of materials for the collider models
    [SerializeField] GameObject ColliderMeshParent;     //The parent that all the static created collider models get assigned to

    System.Random rand;
    List<GameObject> NonStaticColliderModels;           //List of all the non-static collider models
    List<GameObject> SceneObjectsWithCollider;          //List of all valid objects with a collider
    List<GameObject> SceneObjectsWithRenderer;          //List of all valid objects with a renderer
    readonly Collider[] Colliders = {new BoxCollider(), new SphereCollider(), new CapsuleCollider(), new MeshCollider()};    //Array of collider definitions


    //Listeners to handle changing of objects at runtime
    private void OnEnable()
    {
        Events.instance.AddListener<GameObjectCreated>(OnGameObjectCreated);
        Events.instance.AddListener<GameObjectDeleted>(OnGameObjectDeleted);
    }
    private void OnDisable()
    {
        Events.instance.RemoveListener<GameObjectCreated>(OnGameObjectCreated);
        Events.instance.RemoveListener<GameObjectDeleted>(OnGameObjectDeleted);
    }

    //TODO: Handle collider view when new objects are created (maybe create a custom EventHandler)
    void Start()
    {
        Debug.Log("Colliders - Started");
        if (devMode)
        {
            Debug.Log("Colliders - Started Loading");
            rand = new System.Random();
            SceneObjectsWithCollider = new List<GameObject>();
            SceneObjectsWithRenderer = new List<GameObject>();
            NonStaticColliderModels = new List<GameObject>();

            //Get all objects in the scene and assign them to the appropiate list depending on the component

            foreach(GameObject sObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (!IgnoredParents.Contains(sObject))
                {
                    foreach (Transform child in sObject.GetComponentsInChildren<Transform>(true))
                    {
                        //Debug.Log(child.name + " - " + HasComponent<Collider>(child.gameObject));
                        if (HasComponent<Collider>(child.gameObject))
                        {
                            //Debug.Log(child.GetComponent<Collider>().GetType());
                            SceneObjectsWithCollider.Add(child.gameObject);
                        }
                        if (HasComponent<MeshRenderer>(child.gameObject))
                        {
                            SceneObjectsWithRenderer.Add(child.gameObject);
                        }
                    }
                }
            }
            Debug.Log("Colliders - Finished Loading");
            Debug.Log("Colliders - Started Assigning Models");
            foreach (GameObject colliderParent in SceneObjectsWithCollider)
            {
                CreateCollider(colliderParent);
            }
            Debug.Log("Colliders - Finished Assigning Models");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Debug") && devMode)
        {
            isActive = !isActive;
            SwitchViewModels(isActive);
        }
    }

    void OnGameObjectCreated(GameObjectCreated e)
    {
        GameObject CreatedGameObject = e.CreatedGameObject;
        Debug.Log("Game Object Created - " + CreatedGameObject.name);
        if (HasComponent<MeshRenderer>(CreatedGameObject) && !SceneObjectsWithRenderer.Contains(CreatedGameObject))
        {
            SceneObjectsWithRenderer.Add(CreatedGameObject);
        }
        CreateCollider(CreatedGameObject);

        //Temporary solution, write a more efficient solution that would just update the colliders;

        if (isActive)
        {
            ReloadColliderView();
        }
    }

    void OnGameObjectDeleted(GameObjectDeleted e)
    {
        GameObject DeletedGameObject = e.DeletedGameObject;

        if (HasComponent<Collider>(DeletedGameObject))
        {
            if (DeletedGameObject.layer == 7)    //Non-static
            {
                GameObject objectToDelete = null;
                bool hasRenderer = false;
                foreach (GameObject colliderObject in NonStaticColliderModels)
                {
                    if (colliderObject.transform.position == DeletedGameObject.transform.position
                       && colliderObject.transform.rotation == DeletedGameObject.transform.rotation
                       && colliderObject.transform.lossyScale == DeletedGameObject.transform.lossyScale)
                    {
                        objectToDelete = colliderObject;
                        if (SceneObjectsWithRenderer.Contains(DeletedGameObject))
                        {
                            hasRenderer = true;
                        }
                        break;
                    }
                }
                if (hasRenderer)
                {
                    SceneObjectsWithRenderer.Remove(DeletedGameObject);
                }
                if (objectToDelete != null)
                {
                    NonStaticColliderModels.Remove(objectToDelete);
                    Destroy(DeletedGameObject);
                    //Destroy(objectToDelete);
                }
            }
            else
            {
                Destroy(DeletedGameObject);
            }
        }

        //Temporary solution, write a more efficient solution that would just update the colliders;
        if (isActive)
        {
            ReloadColliderView();
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            SwitchViewModels(isActive);
        }
    }
    void SwitchViewModels(bool targetMode)  //0 = normal view, 1 = collider view
    {
        Debug.Log("Collider view - " + targetMode);

        foreach(GameObject gObj in SceneObjectsWithRenderer)
        {
            gObj.GetComponent<MeshRenderer>().enabled = !targetMode;
        }
        foreach(Transform gObj in ColliderMeshParent.GetComponentInChildren<Transform>(true))
        {
            gObj.GetComponent<MeshRenderer>().enabled = targetMode;
        }
        foreach(GameObject gObj in NonStaticColliderModels)
        {
            gObj.GetComponent<MeshRenderer>().enabled = targetMode;
        }
    }
    void ReloadColliderView()
    {
        SwitchViewModels(!isActive);
        SwitchViewModels(isActive);
    }
    void CreateCollider(GameObject colliderParent)  //Creates the collider based on the input object
    {
        GameObject newVisibleCollider = null;
        bool isValid = true;
        bool isBox = false;
        switch (Array.FindIndex(Colliders, c => c.GetType() == colliderParent.GetComponent<Collider>().GetType()))  //Gets the ID of the current collider type
        {
            case 0:             //BoxCollider
                newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                isBox = true;
                Destroy(newVisibleCollider.GetComponent<Collider>());
                break;
            case 1:             //SphereCollider
                newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(newVisibleCollider.GetComponent<Collider>());
                break;
            case 2:             //CapsuleCollider
                newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                Destroy(newVisibleCollider.GetComponent<Collider>());
                break;
            case 3:             //MeshCollider
                newVisibleCollider = new GameObject();
                newVisibleCollider.AddComponent<MeshFilter>();
                newVisibleCollider.GetComponent<MeshFilter>().mesh = colliderParent.GetComponent<MeshCollider>().sharedMesh;            //Gets the original mesh, even if a collider is set to be convex, resulting in inaccurate mesh display.
                                                                                                                                        //However there seems to be no way to access the convex mesh via scripts, so I'm out of luck
                newVisibleCollider.AddComponent<MeshRenderer>();
                break;
            default:
                Debug.Log("Collider view - Unrecognized collider in: " + colliderParent.name);
                isValid = false;
                break;
        }
        if (isValid)
        {
            newVisibleCollider.name = colliderParent.name;
            newVisibleCollider.transform.position = colliderParent.transform.position;
            if (isBox)
                newVisibleCollider.transform.localScale = new Vector3(colliderParent.transform.localScale.x * colliderParent.GetComponent<BoxCollider>().size.x, colliderParent.transform.localScale.y * colliderParent.GetComponent<BoxCollider>().size.y, colliderParent.transform.localScale.z * colliderParent.GetComponent<BoxCollider>().size.z);
            else
                newVisibleCollider.transform.localScale = colliderParent.transform.localScale;
            newVisibleCollider.transform.rotation = colliderParent.transform.rotation;
            newVisibleCollider.GetComponent<MeshRenderer>().material = Materials[rand.Next(Materials.Length)];
            newVisibleCollider.GetComponent<MeshRenderer>().enabled = false;

            if (colliderParent.layer == 7)  //Original object is non-static
            {
                NonStaticColliderModels.Add(newVisibleCollider);
                newVisibleCollider.transform.SetParent(colliderParent.transform);
                if (isBox)
                    newVisibleCollider.transform.localScale = new Vector3(colliderParent.GetComponent<BoxCollider>().size.x, colliderParent.GetComponent<BoxCollider>().size.y, colliderParent.GetComponent<BoxCollider>().size.z);
                else
                    newVisibleCollider.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                newVisibleCollider.transform.SetParent(ColliderMeshParent.transform);
            }
        }
    }
    bool HasComponent <T>(GameObject inputObject) where T:Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>()!=null;
    }
}