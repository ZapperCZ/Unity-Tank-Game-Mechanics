using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [SerializeField] int FPSLimit = 60;
    void Start()
    {
        Application.targetFrameRate = FPSLimit;
    }

}
