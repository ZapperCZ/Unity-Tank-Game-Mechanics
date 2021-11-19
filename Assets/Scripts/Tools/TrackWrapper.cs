using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWrapper : MonoBehaviour
{
    [SerializeField] Transform TrackParent;
    [SerializeField] Transform Sprocket;
    [SerializeField] Transform Idler;
    [SerializeField] Transform RoadWheelParent;
    [SerializeField] Transform ReturnRollerParent;

    List<Transform> Wheels;
    Transform[,] NeighbouringWheels;                            //[index, Neighbouring Wheels]
                                                                //By using index of same value we can get the parent wheel from Wheels and it's neighbouring wheels from NeigbouringWheels
    private void Start()        //Currently won't work when changing wheel transforms at runtime
    {
        Wheels = new List<Transform>();
        PutAllWheelsIntoAList(Wheels);
        NeighbouringWheels = new Transform[Wheels.Count, 2];
        Transform[] tempArray;

        for(int i = 0; i < Wheels.Count; i++)
        {
            tempArray = GetNeighbourWheels(Wheels[i]);
            NeighbouringWheels[i, 0] = tempArray[0];
            NeighbouringWheels[i, 1] = tempArray[1];

            //Debug.Log(@$"Track Wrapper - {transform.name} - {Wheels[i].name}: {NeighbouringWheels[i,0].name}, {NeighbouringWheels[i,1].name}");
        }

    }

    void PutAllWheelsIntoAList(List<Transform> TargetList)  //Takes the assigned wheel transforms and puts them into a single list
    {
        TargetList.Add(Sprocket);
        TargetList.Add(Idler);
        if(RoadWheelParent != null)
        {
            foreach (Transform Axle in RoadWheelParent.transform)
            {
                TargetList.Add(Axle.GetChild(0));   //Get the wheel from the axle and add it into the list
            }
        }
        if (ReturnRollerParent != null)
        {
            foreach (Transform Axle in ReturnRollerParent.transform)
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
