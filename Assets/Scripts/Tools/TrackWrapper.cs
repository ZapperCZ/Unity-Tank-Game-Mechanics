using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWrapper : MonoBehaviour
{
    [SerializeField] Transform TrackParent;
    [SerializeField] Transform Sprocket;
    [SerializeField] Transform Idler;
    [SerializeField] Transform RoadWheelParent;
    [SerializeField] bool hasReturnRollers;
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

        Debug.Log($"Track Wrapper - {transform.name} - {Wheels.Count}");

        for (int i = 0; i < Wheels.Count; i++)
        {
            tempArray = GetClosestWheels(Wheels[i]);
            NeighbouringWheels[i, 0] = tempArray[0];
            NeighbouringWheels[i, 1] = tempArray[1];

            if (hasReturnRollers)
            {
                if (Wheels[i].GetComponent<Wheel>().WheelType == Wheel.TypeOfWheel.Sprocket)
                {
                    NeighbouringWheels[i, 1] = Idler;
                }
                else if (Wheels[i].GetComponent<Wheel>().WheelType == Wheel.TypeOfWheel.Idler)
                {
                    NeighbouringWheels[i, 1] = Sprocket;
                }
            }


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

    Transform[] GetClosestWheels(Transform referenceWheel)      //Returns the 2 closest wheels to the input wheel
    {
        Transform[] ClosestWheels = new Transform[2];             //0 - Closest wheel, 1 - Second closest wheel
        
        //TODO: Create a method that takes 2 wheels as an input and outputs whether the wheels can be connected or not
        //TODO: Move the special condition for sprocket and idler into here

        float minDist = float.MaxValue;
        float minDist2 = float.MaxValue;
        float tempDist;
        Vector3 wheelPos = referenceWheel.position;

        foreach(Transform Wheel in Wheels)
        {
            //Since there are no wheels to compare distance to on the first iteration, the first found wheel is the closest one
            if (ClosestWheels[0] == null) 
            {
                ClosestWheels[0] = Wheel;
                minDist = Vector3.Distance(referenceWheel.position,Wheel.position);     //Distance between the closest wheel and the input wheel
                continue;
            }
            //Runs on the second iteration
            if (ClosestWheels[1] == null)
            {
                tempDist = Vector3.Distance(referenceWheel.position, Wheel.position);
                if (tempDist < minDist)
                {
                    //The new wheel is closer than the previous one
                    //Set the previously closest wheel to now be the second closest one
                    ClosestWheels[1] = ClosestWheels[0];
                    ClosestWheels[0] = Wheel;
                    minDist2 = minDist;
                    minDist = tempDist;
                    continue;
                }
                else
                {
                    //The new wheel is further than the previous one
                    //Set the new wheel as the second closest one
                    ClosestWheels[1] = Wheel;
                    minDist2 = tempDist;
                    continue;
                }
            }
        }

        return ClosestWheels;
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
