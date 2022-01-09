using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionForce = 10f;

    void OnCollisionEnter(Collision collision)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        GameObject obj;
        foreach(Collider c in nearbyObjects)
        {
            obj = c.transform.gameObject;
            if (HasComponent<Rigidbody>(obj))
            {
                obj.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
