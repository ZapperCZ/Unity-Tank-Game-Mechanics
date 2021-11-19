using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWrapper : MonoBehaviour
{
    [SerializeField] Transform TrackParent;
    [SerializeField] Transform Sprocket;
    [SerializeField] Transform Idler;
    [SerializeField] Transform RoadWheel_Parent;
    [SerializeField] Transform ReturnRoller_Parent;

    List<Transform> Wheels;
    Dictionary<Transform, Transform> NeighbouringWheels;            //Can't use a dictionary since we need the key (MainWheel) to have 2 neigbour wheels
                                                                    //TODO: Use a 2D array instead

    private void Start()
    {
        PutAllWheelsIntoAList(Wheels);
        Transform[] tempArray = new Transform[2];
        KeyValuePair<Transform, Transform> WheelPair;               //<MainWheel, NeighbouringWheel>

        foreach(Transform Wheel in Wheels)
        {
            tempArray = GetNeighbourWheels(Wheel);
            if (!IsTransformInDictionary(Wheel, NeighbouringWheels))            //Won't work since we need each of the wheels to be twice in the list
            {
                WheelPair = new KeyValuePair<Transform, Transform>(Wheel, tempArray[0]);
            }
        }
    }

    void PutAllWheelsIntoAList(List<Transform> TargetList)  //Takes the assigned wheel transforms and puts them into a single list
    {
        TargetList.Add(Sprocket);
        TargetList.Add(Idler);
        if(RoadWheel_Parent != null)
        {
            foreach (Transform Axle in RoadWheel_Parent.transform)
            {
                TargetList.Add(Axle.GetChild(0));   //Get the wheel from the axle and add it into the list
            }
        }
        if (ReturnRoller_Parent != null)
        {
            foreach (Transform Axle in ReturnRoller_Parent.transform)
            {
                TargetList.Add(Axle.GetChild(0));   //Get the wheel from the axle and add it into the list
            }
        }
    }

    Transform[] GetNeighbourWheels(Transform referenceWheel)
    {
        Transform[] NeighbourWheels = new Transform[2];


        return NeighbourWheels;
    }


    

    Line CreateOuterTangent(Circle circle_A, Circle circle_B, bool isUpper)     //isUpper determines whether the tangent should be made on the "upper" sides of the circles or on the "lower" sides
    {
        //TODO: Finish this
        Line tangent = new Line();
        return tangent;
    }
    bool IsTransformInDictionary(Transform transformToCheck, Dictionary<Transform, Transform> dictionaryToCheck)    //Checks whether a dictionary contains the input value, either as a key or as a value
    {
        return dictionaryToCheck.ContainsKey(transformToCheck) || dictionaryToCheck.ContainsValue(transformToCheck);
    }
}
