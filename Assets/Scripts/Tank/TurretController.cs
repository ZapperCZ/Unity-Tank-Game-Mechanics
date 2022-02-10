using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] Transform Camera;
    [SerializeField] Transform Test;
    [SerializeField] Transform Gun;
    [SerializeField] float angleDifferenceThreshold = 4;        //The angle threshold where the component starts deecreasing it's velocity
    [SerializeField] float maxTurretSlewingSpeed = 15f;
    [SerializeField] float maxGunElevationSpeed = 10f;

    Vector3 TargetPoint;
    Vector2 TurretDirection;
    Vector2 TargetDirectionFromTurret;
    Vector2 GunDirection;
    Vector2 TargetDirectionFromGun;

    float TurretAngleToTarget;
    float GunAngleToTarget;

    float gunMagnitude = 1f;
    float turretMagnitude = 1f;

    JointMotor TurretDrive;
    JointMotor GunDrive;

    private void Awake()
    {
        TurretDrive = transform.GetComponent<HingeJoint>().motor;
        GunDrive = Gun.GetComponent<HingeJoint>().motor;
    }
    void OnDisable()
    {
        TurretDrive.targetVelocity = 0;
        GunDrive.targetVelocity = 0;

        transform.GetComponent<HingeJoint>().motor = TurretDrive;
        Gun.GetComponent<HingeJoint>().motor = GunDrive;
    }
    void Update()
    {
        GetTargetPoint();

        TurretDirection = new Vector2(transform.forward.x,transform.forward.z);
        TargetDirectionFromTurret = new Vector2(TargetPoint.x - transform.position.x, TargetPoint.z - transform.position.z);

        GunDirection = new Vector2(Mathf.Abs(Gun.forward.x), Gun.forward.y);
        TargetDirectionFromGun = new Vector2(Mathf.Abs(TargetPoint.x - Gun.position.x), (TargetPoint.y - Gun.position.y));        //The sign of the X component is omitted to not mess with sign of a resulting angle. The Y component is inverted because idk, just doesn't work without it

        TurretAngleToTarget = AngleBetweenVector2(TurretDirection, TargetDirectionFromTurret);
        GunAngleToTarget = AngleBetweenVector2(GunDirection, TargetDirectionFromGun);

        if(Mathf.Abs(GunAngleToTarget) < angleDifferenceThreshold)
        {
            gunMagnitude = normalizeNumber(GunAngleToTarget, angleDifferenceThreshold);
        }
        else
        {
            gunMagnitude = 1f;
        }
        
        if(Mathf.Abs(TurretAngleToTarget) < angleDifferenceThreshold)
        {
            turretMagnitude = normalizeNumber(TurretAngleToTarget, angleDifferenceThreshold);
        }
        else
        {
            turretMagnitude = 1f;
        }

        if (TurretAngleToTarget > 0)            //Player is aiming to the turret left
        {
            TurretDrive.targetVelocity = -maxTurretSlewingSpeed * turretMagnitude;
            transform.GetComponent<HingeJoint>().motor = TurretDrive;
        }
        else if (TurretAngleToTarget < 0)       //Player is aiming to the turret right
        {
            TurretDrive.targetVelocity = maxTurretSlewingSpeed * turretMagnitude;
            transform.GetComponent<HingeJoint>().motor = TurretDrive;
        }

        if(GunAngleToTarget > 0)         //Player is aiming above the gun
        {
            GunDrive.targetVelocity = -maxGunElevationSpeed * gunMagnitude;
            Gun.GetComponent<HingeJoint>().motor = GunDrive;
        }
        else if(GunAngleToTarget < 0)    //Player is aiming below the gun
        {
            GunDrive.targetVelocity = maxGunElevationSpeed * gunMagnitude;
            Gun.GetComponent<HingeJoint>().motor = GunDrive;
        }

        //TurretPosition = Vector2.zero;
    }

    void GetTargetPoint()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.position, Camera.forward, 1000f);

        List<RaycastHit> hitList = hits.OrderBy(x => Vector2.Distance(Camera.GetComponent<VehicleCameraController>().FocusPoint.position, x.point)).ToList();

        foreach (RaycastHit hit in hitList)
        {
            if (!hit.transform.CompareTag("Player Controlled"))
            {
                TargetPoint = hit.point;
                Test.position = TargetPoint;
                return;
            }
        }
    }
    float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
        float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
        return Vector2.Angle(vec1, vec2) * sign;
    }
    float normalizeNumber(float num, float max)
    {
        return Mathf.Abs(num / max);
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
