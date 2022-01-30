using UnityEngine;

[ExecuteInEditMode]
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

    bool changed = false;

    void OnEnable()
    {
        CheckScriptRequirements();
    }

    void OnValidate()
    {
        CheckScriptRequirements();
        changed = true;
    }

    void Update()
    {
        if (changed)
        {
            switch (TypeOfCollisionAdjustment)
            {
                //Note: Physics.IgnoreLayerCollision disables collisions between 2 layers, not a collider and a layer. Either new layer has to be created for the object at runtime, which can mess with other scripts,
                //      or all of the objects contained within the input layer have to be looped through, disabling collisions with them by using Physics.IgnoreCollisions. This will be much more performance costly

                case AdjustmentType.IgnoreCollisionsWithObjects:        //Select all layers, enable collisions with them, disable collisions with input objects

                    break;
                case AdjustmentType.IgnoreCollisionsWithLayers:         //Select all layers, enable collisions with them, disable collisions with input layers

                    break;
                case AdjustmentType.CollideOnlyWithObjects:             //Select all layers, disable collisions with them, enable collisions with input objects

                    break;
                case AdjustmentType.CollideOnlyWithLayers:              //Select all layers, disable collisions with them, enable collisions with input layers
                    break;
            }
        }
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
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
