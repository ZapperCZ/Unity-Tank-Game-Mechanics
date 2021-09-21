using System;
using System.Collections;
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
    Collider[] Colliders = {new BoxCollider(), new SphereCollider(), new CapsuleCollider(), new MeshCollider()};    //Array of collider definitions

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
    private void OnValidate()
    {
        SwitchViewModels(isActive);
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
            gObj.gameObject.GetComponent<MeshRenderer>().enabled = targetMode;
        }
        foreach(GameObject gObj in NonStaticColliderModels)
        {
            gObj.gameObject.GetComponent<MeshRenderer>().enabled = targetMode;
        }
    }
    void CreateCollider(GameObject colliderParent)  //Creates the collider based on the input object
    {
        GameObject newVisibleCollider = null;
        bool isValid = true;
        switch (Array.FindIndex(Colliders, c => c.GetType() == colliderParent.GetComponent<Collider>().GetType()))  //Gets the ID of the current collider type
        {
            case 0:             //BoxCollider
                newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
                newVisibleCollider.GetComponent<MeshFilter>().mesh = colliderParent.GetComponent<MeshCollider>().sharedMesh;
                newVisibleCollider.AddComponent<MeshRenderer>();
                break;
            default:
                Debug.Log("Colliders - Unrecognized collider in: " + colliderParent.name);
                isValid = false;
                break;
        }
        if (isValid)
        {
            newVisibleCollider.name = colliderParent.name;
            newVisibleCollider.transform.position = colliderParent.transform.position;
            newVisibleCollider.transform.localScale = colliderParent.transform.localScale;
            newVisibleCollider.transform.rotation = colliderParent.transform.rotation;
            newVisibleCollider.GetComponent<MeshRenderer>().material = Materials[rand.Next(Materials.Length)];
            newVisibleCollider.GetComponent<MeshRenderer>().enabled = false;

            if (colliderParent.layer == 7)  //Original object is non-static
            {
                NonStaticColliderModels.Add(newVisibleCollider);
                newVisibleCollider.transform.SetParent(colliderParent.transform);
            }
            else
            {
                newVisibleCollider.transform.SetParent(ColliderMeshParent.transform);
            }
        }
    }
    bool HasComponent <T>(GameObject inputObject) where T:Component //Returns whether the input object has a collider or not
    {
        return inputObject.GetComponent<T>()!=null;
    }
}