using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create new wheel type")]
public class SO_WheelComponents : ScriptableObject
//An SO that stores components that should be added to individual wheel types
{
    [SerializeField] WheelType WheelTypeToModify;
    [SerializeField] List<Component> WheelComponents;

    List<Component>[] WheelComponentArray;      //An array of lists, each list corresponds to a wheel type and carries the components that the wheel type needs
                                                //Size of the array is linked to the amount of Wheel types defined by the WheelType enum
    int WheelTypeAmount = 0;                    //Amount of wheel types, updates in OnValidate
    private void OnValidate()
    {
        WheelTypeAmount = Enum.GetValues(typeof(WheelType)).Length;
        if (WheelComponentArray == null)
        {
            WheelComponentArray = new List<Component>[WheelTypeAmount];
            for(int i = 0; i < WheelComponentArray.Length; i++)
            {
                WheelComponentArray[i] = new List<Component>();
            }
        }
        if(WheelComponentArray.Length != WheelTypeAmount)
        {
            //TODO: Change the size of the WheelComponentArray, copy the components from the old array to the new array
        }
        WheelComponentArray[0].Add(new BoxCollider());
        WheelComponents = WheelComponentArray[(int)WheelTypeToModify];
        Debug.Log(WheelComponentArray.Length);
        //if(Enum.GetValues(typeof(TypeOfWheel)).Length)
    }
    void CopyOldComponents()    //Copies over all components of wheel types when a new wheel type is added and the wheel component array is rebuilt
    {

    }
}