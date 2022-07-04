using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new wheel type")]
public class SO_WheelComponents : ScriptableObject
//An SO that stores components that should be added to individual wheel types
{
    [SerializeField] float testA;
    [SerializeField] float testB;
}