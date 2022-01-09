using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] Transform BarrelPosition;
    [SerializeField] GameObject Shell;
    [SerializeField] float force;
    [SerializeField] float reloadTime;
    [SerializeField] float reloadCountdown = 0f;
    bool reloaded = true;

    void Update()
    {
        if(Input.GetButton("Left Click") && reloaded)
        {
            Fire();
        }
        if (!reloaded && reloadCountdown > 0)
        {
            reloadCountdown -= Time.deltaTime;
        }
        if(reloadCountdown < 0)
        {
            reloadCountdown = 0f;
            reloaded = true;
        }
    }
    void Fire()
    {
        GameObject FiredShell = Instantiate(Shell);
        FiredShell.transform.parent = BarrelPosition;
        FiredShell.transform.localPosition = new Vector3(0, 0, FiredShell.transform.localScale.z);
        FiredShell.transform.localRotation = Quaternion.Euler(90, 0, 0);
        FiredShell.GetComponent<Collider>().enabled = true;
        FiredShell.GetComponent<Rigidbody>().AddForce(BarrelPosition.forward.normalized * force);

        reloadCountdown = reloadTime;
        reloaded = false;
    }
}
