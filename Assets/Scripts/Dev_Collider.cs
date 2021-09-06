using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dev_Collider : MonoBehaviour
{
    public bool devMode = true;
    public bool isActive = false;

    public GameObject[] IgnoredParents;
    public GameObject ColliderMeshParent;

    List<GameObject> SceneObjectsWithCollider;
    List<GameObject> SceneObjectsWithRenderer;
    Collider[] Colliders = {new BoxCollider(), new SphereCollider(), new CapsuleCollider(), new MeshCollider()};

    //TODO: Handle collider view when new objects are created
    void Start()
    {
        Debug.Log("Colliders - Started");
        if (devMode)
        {
            Debug.Log("Colliders - Started Loading");
            SceneObjectsWithCollider = new List<GameObject>();
            SceneObjectsWithRenderer = new List<GameObject>();

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
                GameObject newVisibleCollider = new GameObject();
                bool isValid = true;
                switch (Array.FindIndex(Colliders, c => c.GetType() == colliderParent.GetComponent<Collider>().GetType()))
                {
                    case 0:             //BoxCollider
                        newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        break;
                    case 1:             //SphereCollider
                        newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        break;
                    case 2:             //CapsuleCollider
                        newVisibleCollider = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        break;
                    case 3:             //MeshCollider
                        newVisibleCollider.AddComponent<MeshRenderer>();
                        break;
                    default:
                        Debug.Log("Colliders - Unrecognized collider in: "+colliderParent.name);
                        isValid = false;
                        break;
                }
                if (isValid)
                {
                    Destroy(newVisibleCollider.GetComponent<Collider>());
                    newVisibleCollider.transform.SetParent(ColliderMeshParent.transform);
                }
            }
            Debug.Log("Colliders - Finished Assigning Models");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Debug") && devMode)
        {
            isActive = !isActive;
            SwitchViewModels(isActive);
        }
        
    }
    void SwitchViewModels(bool targetMode)  //0 = normal, 1 = colliders
    {
        Debug.Log("Collider view - " + targetMode);
        //Disables all mesh renderers in scene
        foreach(GameObject gObj in SceneObjectsWithRenderer)
        {
            gObj.GetComponent<MeshRenderer>().enabled = !targetMode;
        }
    }
    bool HasComponent <T>(GameObject inputObject) where T:Component
    {
        return inputObject.GetComponent<T>()!=null;
    }
}
