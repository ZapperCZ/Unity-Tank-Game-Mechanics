using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] Transform Camera;
    [SerializeField] Transform Test;
    [SerializeField] Transform Gun;
    [SerializeField] float turretSlewingSpeed = 15f;
    [SerializeField] float gunElevationSpeed = 10f;

    Vector3 TargetPoint;
    Vector2 TurretDirection;
    Vector2 TargetDirectionXZ;
    float GunDirection;
    float TargetDirectionY;

    JointMotor TurretDrive;
    JointMotor GunDrive;

    private void Awake()
    {
        TurretDrive = transform.GetComponent<HingeJoint>().motor;
        GunDrive = Gun.GetComponent<HingeJoint>().motor;
    }
    void Update()
    {
        GetTargetPoint();

        TurretDirection = new Vector2(transform.forward.x,transform.forward.z);
        TargetDirectionXZ = new Vector2(Camera.forward.x, Camera.forward.z);

        GunDirection = Gun.forward.y;
        TargetDirectionY = Camera.forward.y;

        //Debug.Log(GunDirection + " - " + TargetDirectionY);

        //Debug.Log(AngleBetweenVector2(TurretDirection,TargetDirectionXZ) + " - " +  TurretDirection + " - " + TargetDirectionXZ);

        if (AngleBetweenVector2(TurretDirection, TargetDirectionXZ) > 0)            //player is aiming to the turret left
        {
            TurretDrive.targetVelocity = -turretSlewingSpeed;
            transform.GetComponent<HingeJoint>().motor = TurretDrive;
        }
        else if (AngleBetweenVector2(TurretDirection, TargetDirectionXZ) < 0)     //player is aiming to the turret right
        {
            TurretDrive.targetVelocity = turretSlewingSpeed;
            transform.GetComponent<HingeJoint>().motor = TurretDrive;
        }

        if(TargetDirectionY > GunDirection)         //Player is aiming above the gun
        {
            GunDrive.targetVelocity = -gunElevationSpeed;
            Gun.GetComponent<HingeJoint>().motor = GunDrive;
        }
        else if(TargetDirectionY < GunDirection)    //Player is aiming below the gun
        {
            GunDrive.targetVelocity = gunElevationSpeed;
            Gun.GetComponent<HingeJoint>().motor = GunDrive;
        }

        //TurretPosition = Vector2.zero;
    }

    void GetTargetPoint()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.position, Camera.forward, 1000f);

        List<RaycastHit> hitList = new List<RaycastHit>(hits);
        hitList.Sort((x, y) => y.collider.gameObject.layer.CompareTo(x.collider.gameObject.layer));

        foreach (RaycastHit hit in hitList)
        {
            if (!hit.transform.CompareTag("Player Controlled"))
            {
                TargetPoint = hit.point;
                Test.position = TargetPoint;
                break;
            }
        }
    }
    float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
        float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
        return Vector2.Angle(vec1, vec2) * sign;
    }
}
