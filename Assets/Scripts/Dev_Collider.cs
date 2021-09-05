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

    List<GameObject> SceneObjectsWithColliders;
    List<GameObject> SceneObjects;
    Collider[] Colliders = {new BoxCollider(), new SphereCollider(), new CapsuleCollider(), new MeshCollider()};

    //TODO: Handle collider view when new objects are created
    void Start()
    {
        Debug.Log("Colliders - Started");
        if (devMode)
        {
            Debug.Log("Colliders - Started Loading");
            SceneObjectsWithColliders = new List<GameObject>();
            SceneObjects = new List<GameObject>();

            foreach(GameObject sObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                //Add object to the list if it has a collider
                if (!IgnoredParents.Contains(sObject))
                {
                    /*
                    Debug.Log("----Parent----");
                    Debug.Log(sObject.name);
                    Debug.Log("----Children----");
                    */
                    foreach (Transform child in sObject.GetComponentsInChildren<Transform>(true))
                    {
                        SceneObjects.Add(child.gameObject);
                        //Debug.Log(child.name + " - " + HasComponent<Collider>(child.gameObject));
                        if (HasComponent<Collider>(child.gameObject))
                        {
                            Debug.Log(child.GetComponent<Collider>().GetType());
                            SceneObjectsWithColliders.Add(child.gameObject);
                        }
                    }
                }
            }
            Debug.Log("Colliders - Finished Loading");
            Debug.Log("Colliders - Started Assigning Models");
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

    }
    bool HasComponent <T>(GameObject inputObject) where T:Component
    {
        return inputObject.GetComponent<T>()!=null;
    }
}
