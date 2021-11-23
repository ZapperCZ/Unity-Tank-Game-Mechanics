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
    Line testLine = new Line(new Vector3(0, 1, 0), new Vector3(5, 1, 0));
    LineRenderer LineRenderer;
    Transform[,] NeighbouringWheels;                            //[index, Neighbouring Wheels]
                                                                //By using index of same value we can get the parent wheel from Wheels and it's neighbouring wheels from NeigbouringWheels
    private void Start()        //Currently won't work when changing wheel transforms at runtime
    {
        LineRenderer = gameObject.AddComponent<LineRenderer>();
        testLine = new Line(new Vector3(0, 1, 0), new Vector3(5, 1, 0));
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

            if (!hasReturnRollers)
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
            
            Debug.Log(@$"Track Wrapper - {transform.name} - {Wheels[i].name}: {NeighbouringWheels[i,0].name}, {NeighbouringWheels[i,1].name}");
        }

    }
    private void OnGUI()
    {
        LineRenderer.startColor = Color.red;
        LineRenderer.endColor = Color.red;

        // set width of the renderer
        LineRenderer.startWidth = 0.3f;
        LineRenderer.endWidth = 0.3f;

        LineRenderer.SetPosition(0, testLine.pointA);
        LineRenderer.SetPosition(1, testLine.pointB);
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
        //TODO: Move the special condition for sprocket and idler without return rollers into here

        float minDist = float.MaxValue;         //Distance of the closest wheel
        float minDist2 = float.MaxValue;        //Distance of the second closest wheel
        float tempDist;
        Vector3 wheelPos = referenceWheel.position;
            
        foreach(Transform NewWheel in Wheels)
        {
            if (AreWheelsConnectable(referenceWheel.GetComponent<Wheel>(), NewWheel.GetComponent<Wheel>()))
            {
                //Runs on the first iteration
                if (ClosestWheels[0] == null)
                {
                    //Since the new wheel is the first one found, it is also the closest one
                    ClosestWheels[0] = NewWheel;
                    minDist = Vector3.Distance(wheelPos, NewWheel.position);     //Distance between the closest wheel and the input wheel
                    continue;
                }
                //Runs on the second iteration
                if (ClosestWheels[1] == null)
                {
                    tempDist = Vector3.Distance(wheelPos, NewWheel.position);
                    if (tempDist < minDist)
                    {
                        //The new wheel is closer than the previous one
                        //Set the previously closest wheel to now be the second closest one
                        ClosestWheels[1] = ClosestWheels[0];
                        ClosestWheels[0] = NewWheel;
                        minDist2 = minDist;
                        minDist = tempDist;
                        continue;
                    }
                    else
                    {
                        //The new wheel is further than the previous one
                        //Set the new wheel as the second closest one
                        ClosestWheels[1] = NewWheel;
                        minDist2 = tempDist;
                        continue;
                    }
                }

                //The new wheel and the input wheel can theoretically have track between them
                tempDist = Vector3.Distance(wheelPos, NewWheel.position);
                if (tempDist < minDist)
                {
                    //New wheel is closer than the closest wheel
                    ClosestWheels[0] = NewWheel;
                    minDist = tempDist;
                    continue;
                }
                //Will run only when the wheel is further than the closest wheel but closer than the second closest wheel
                if (tempDist < minDist2)
                {
                    //New wheel is closer than the second closest wheel
                    ClosestWheels[1] = NewWheel;
                    minDist2 = tempDist;
                    continue;
                }
            }
        }

        return ClosestWheels;
    }


    bool AreWheelsConnectable(Wheel firstWheel, Wheel secondWheel)
    {
        bool result = false;
        if(firstWheel.gameObject == secondWheel.gameObject)
        {
            //The wheels are the same, a wheel can't connect to itself
            return result;
        }
        switch (firstWheel.WheelType)
        {
            //Ordered by the probability of the wheel being the corresponding type (highest to lowest)
            case Wheel.TypeOfWheel.Roadwheel:
                result = secondWheel.WheelType != Wheel.TypeOfWheel.ReturnRoller;
                break;
            case Wheel.TypeOfWheel.ReturnRoller:
                result = secondWheel.WheelType != Wheel.TypeOfWheel.Roadwheel;
                break;
            case Wheel.TypeOfWheel.Sprocket:
                //Doesn't handle the special case where there are no return rollers, this is handled elsewhere
                result = secondWheel.WheelType != Wheel.TypeOfWheel.Idler && secondWheel.WheelType != Wheel.TypeOfWheel.Sprocket;
                break;
            case Wheel.TypeOfWheel.Idler:
                //Doesn't handle the special case where there are no return rollers, this is handled elsewhere
                result = secondWheel.WheelType != Wheel.TypeOfWheel.Sprocket && secondWheel.WheelType != Wheel.TypeOfWheel.Idler;
                break;
        }
        return result;
    }

    Line CreateOuterTangent(Circle circle_A, Circle circle_B, bool isUpper)     //isUpper determines whether the tangent should be made on the "upper" sides of the circles or on the "lower" sides
    {
        //TODO: Finish this
        Line tangent = new Line();
        return tangent;
    }
    bool HasComponent<T>(GameObject inputObject) where T : Component //Returns whether the input object has a component or not
    {
        return inputObject.GetComponent<T>() != null;
    }
}
