using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionForce = 10f;

    Vector3 previousPosition = new Vector3(0,0,0);

    void FixedUpdate()
    {
        HighSpeedCollisionDetection();
    }

    void HighSpeedCollisionDetection()
    {
        Ray ray = new Ray(transform.position, transform.position - previousPosition);

        if (Physics.Raycast(ray,out RaycastHit hit, Vector3.Distance(transform.position, previousPosition)))
        {
            TriggerExplosion(hit.point);
        }

        previousPosition = transform.position;
    }
    void TriggerExplosion(Vector3 CurrentPosition)
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        GameObject obj;
        foreach (Collider c in nearbyObjects)
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
