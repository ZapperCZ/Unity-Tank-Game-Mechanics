using System.Collections.Generic;
using UnityEngine;

public class CollisionAdjuster : MonoBehaviour
{
    //A script that adjusts whether collisions should occur between the object the input objects and the parent object of the script.

    private enum AdjustmentType
    {
        IgnoreCollisionsWithObjects,
        IgnoreCollisionsWithLayers,
        CollideOnlyWithObjects,
        CollideOnlyWithLayers
    }

    [SerializeField] AdjustmentType TypeOfCollisionAdjustment = new AdjustmentType();
    [SerializeField] bool adjustChildrenOfObjects;
    [Header("Input")]
    [SerializeField] GameObject[] inputObjects;
    [SerializeField] LayerMask inputLayers;

    void OnValidate()
    {
        CheckScriptRequirements();
    }

    void OnEnable()
    {
        CheckScriptRequirements();

        LayerMask allLayers = ~0;                   //~ is the NOT binary operator in C# -> ~0 gets the inverse of 0 - which is an int's max value
        Collider[] SceneColliders;

        if (!Application.isPlaying)
        {
            Debug.LogWarning(this.GetType().Name + " - " + this.name + " - Application isn't in playmode, stopping execution");
            return;
        }

        switch (TypeOfCollisionAdjustment)
        {
            //Note: Physics.IgnoreLayerCollision disables collisions between 2 layers, not a collider and a layer. Either new layer has to be created for the object at runtime, which can mess with other scripts,
            //      or all of the objects contained within the input layer have to be looped through, disabling collisions with them by using Physics.IgnoreCollisions. This will be much more performance costly

            case AdjustmentType.IgnoreCollisionsWithObjects:        //Select all objects, enable collisions with them, disable collisions with input objects

                SceneColliders = (Collider[])FindObjectsOfType(typeof(Collider));

                //Enables collisions for all colliders in the scene
                foreach(Collider colliderToAdjust in SceneColliders)
                {
                    Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), colliderToAdjust, false);
                }

                //Disables collisions for the target objects
                foreach(GameObject targetObject in inputObjects)
                {
                    AdjustCollisions(this.gameObject, targetObject, true);
                }

                break;
            case AdjustmentType.IgnoreCollisionsWithLayers:         //Select all layers, enable collisions with them, disable collisions with input layers
                Debug.LogError(this.GetType().Name + " - " + this.name + " - Not implemented yet");
                this.enabled = false;
                break;
            case AdjustmentType.CollideOnlyWithObjects:             //Select all objects, disable collisions with them, enable collisions with input objects

                //Gets all colliders within the scene, including inactive
                SceneColliders = (Collider[])FindObjectsOfType(typeof(Collider), true);

                //Iterates through them and disables collisions with them
                foreach (Collider colliderToAdjust in SceneColliders)
                {
                    Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), colliderToAdjust, true);
                }
                
                //Enables collisions for the target objects
                foreach(GameObject targetObject in inputObjects)
                {
                    AdjustCollisions(this.gameObject, targetObject, false);
                }

                break;
            case AdjustmentType.CollideOnlyWithLayers:              //Select all layers, disable collisions with them, enable collisions with input layers
                Debug.LogError(this.GetType().Name + " - " + this.name + " - Not implemented yet");
                this.enabled = false;
                break;
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
    
    //Maybe have a central, one instance script that would work as a cache for all lists of objects in a layer. It would also keep track of when the list of objects was
    //last updated so that the script could either get the cached value, or update the list by getting the objects all over again.

    GameObject[] GetAllObjectsInLayer(string layerName)
    {
        GameObject[] SceneObjects = (GameObject[]) FindObjectsOfType(typeof(GameObject), true);
        List<GameObject> ResultList = new List<GameObject>();

        for(int i = 0; i < SceneObjects.Length; i++)
        {
            if (SceneObjects[i].layer == LayerMask.NameToLayer(layerName))
                ResultList.Add(SceneObjects[i]);
        }

        return ResultList.ToArray();
    }
    GameObject[] GetAllObjectsInLayer(int layer)
    {
        GameObject[] SceneObjects = (GameObject[])FindObjectsOfType(typeof(GameObject));
        List<GameObject> ResultList = new List<GameObject>();

        for (int i = 0; i < SceneObjects.Length; i++)
        {
            if (SceneObjects[i].layer == layer)
                ResultList.Add(SceneObjects[i]);
        }

        return ResultList.ToArray();
    }
    void AdjustCollisions(GameObject InputObject, GameObject TargetObject, bool ignoreCollisions)
    {
        if (HasComponent<Collider>(InputObject) && HasComponent<Collider>(TargetObject))
        {
            Physics.IgnoreCollision(InputObject.GetComponent<Collider>(), TargetObject.GetComponent<Collider>(), ignoreCollisions);
        }
        if (adjustChildrenOfObjects)
        {
            foreach (Transform child in TargetObject.transform)
            {
                AdjustCollisions(InputObject, child.gameObject, ignoreCollisions);
            }
        }
    }
    void CheckScriptRequirements()
    {
        if (!HasComponent<Collider>(transform.gameObject))
        {
            Debug.LogError(this.GetType().Name + " - " + this.name + " - Required components not found, disabling script. Make sure that the script has all GameObjects assigned");
            this.enabled = false;
        }
    }
}
