using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dev_Collider : MonoBehaviour
{
    public bool devMode = true;
    public bool isActive = false;

    public GameObject CollidersParent;
    public GameObject DevStuff;

    List<GameObject> SceneObjects;

    //public Type[] ColliderArray = {BoxCollider, typeof(MeshCollider)};

    //TODO: Handle collider view when new objects are created
    void Start()
    {
        Debug.Log("Colliders - Started");
        if (devMode)
        {
            /*
            foreach(Type collider in ColliderArray)
            {
                Debug.Log(collider.GetType());
            }
            */

            Debug.Log("Colliders - Started Loading");
            SceneObjects = new List<GameObject>();

            //FIX: Currently only gets root objects in the scene, find a way to get the children as well
            foreach(GameObject sObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                //Add object to the list if it has a collider
                if (sObject.GetComponent<Transform>()!=CollidersParent.GetComponent<Transform>() || sObject.GetComponent<Transform>() != DevStuff.GetComponent<Transform>())
                {
                    Debug.Log("----Parent----");
                    Debug.Log(sObject.name);
                    Debug.Log("----Children----");
                    foreach (Transform child in sObject.GetComponentsInChildren<Transform>(true))
                    {
                        Debug.Log(child.name);
                    }
                }


                /*
                Component[] objectComponents = sObject.GetComponents<Component>();
                foreach(Component c in objectComponents)
                {
                    Debug.Log(sObject.name + " - " + c.GetType());
                    if (ColliderArray.Contains(c.GetType()))
                    {
                        Debug.Log(sObject.name + " - Has Collider");
                        break;
                    }
                }

                //TODO: Create an array of colliders to check and then iterate through them checking which one the object has
                if (HasComponent<BoxCollider>(sObject))
                {
                    Debug.Log(sObject.name + " - Box Collider");
                }
                */
            }
            Debug.Log("Colliders - Finished Loading");
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
