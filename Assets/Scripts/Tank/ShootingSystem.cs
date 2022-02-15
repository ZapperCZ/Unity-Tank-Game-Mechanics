using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] Transform BarrelPosition;
    [SerializeField] GameObject Shell;
    [SerializeField] float force;
    [SerializeField] float recoilForce;
    [SerializeField] float reloadTime;
    [SerializeField] float reloadCountdown = 0f;
    [SerializeField] bool fire = false;

    bool reloaded = true;

    void Update()
    {
        if((Input.GetButton("Left Click") && reloaded) || fire)
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

        Vector3 Recoil = transform.forward * -1 * recoilForce;
        transform.GetComponent<Rigidbody>().AddForce(Recoil);

        reloadCountdown = reloadTime;
        reloaded = false;
    }
}
