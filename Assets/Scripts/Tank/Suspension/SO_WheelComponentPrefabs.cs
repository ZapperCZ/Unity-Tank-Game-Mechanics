using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new WCP SO")]
public class SO_WheelComponentPrefabs : ScriptableObject
//An SO that stores components that should be added to individual wheel types
{
    [SerializeField] WheelType WheelTypeToModify;               //An enum that sets which wheel is being edited; changed by user
    [SerializeField] GameObject WheelComponentPrefab;           //The prefab containing components for the currently selected wheel typed

    [HideInInspector] public GameObject[] WheelComponentPrefabArray;      //A list of prefabs that contain the components for a wheel to use

    GameObject prevWCP;
    int WheelTypeAmount = 0;                    //Amount of wheel types, updates in OnValidate
    private void OnValidate()
    {
        WheelTypeAmount = Enum.GetValues(typeof(WheelType)).Length;     //The amount of wheel types (size of the WheelType enum)
        if (WheelComponentPrefabArray.Length != WheelTypeAmount)
        {
            WheelComponentPrefabArray = new GameObject[WheelTypeAmount];
            prevWCP = WheelComponentPrefab;
        }

        Debug.Log(WheelTypeToModify + " , " + WheelComponentPrefabArray.Length);
        if (WheelComponentPrefab != prevWCP)     //A new prefab has been assigned by the user
        {
            WheelComponentPrefabArray[(int)WheelTypeToModify] = WheelComponentPrefab;
        }
        else        //Prefab hasn't been changed but OnValidate triggered -> WheelTypeToModify possibly changed
        {
            WheelComponentPrefab = WheelComponentPrefabArray[(int)WheelTypeToModify];
        }

        prevWCP = WheelComponentPrefab;
    }
}