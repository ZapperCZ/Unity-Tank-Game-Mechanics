using System;
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
    [SerializeField] Transform LineRendererParent;
    List<Transform> Wheels;
    List<MathLine> ConnectingLines;
    List<Transform> DrawnWheels;

    MathLine testLine = new MathLine(new Vector3(0, 1, 0), new Vector3(5, 1, 0));
    //LineRenderer LineRenderer;
    Transform[,] NeighbouringWheels;                            //[index, Neighbouring Wheels]
                                                                //By using index of same value we can get the parent wheel from Wheels and it's neighbouring wheels from NeigbouringWheels
    private void Start()        //Currently won't work when changing wheel transforms at runtime
    {
        #region lineRenderer
        LineRenderer LineRenderer = gameObject.AddComponent<LineRenderer>();
        LineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        LineRenderer.startColor = new Color(1f, 0.53f, 0, 1f);
        LineRenderer.endColor = new Color(1f, 0.53f, 0, 1f);

        // set width of the renderer
        LineRenderer.startWidth = 0.05f;
        LineRenderer.endWidth = 0.05f;
        LineRenderer.loop = true;

        //LineRenderer.SetPosition(0, testLine.pointA);
        //LineRenderer.SetPosition(1, testLine.pointB);
        #endregion

        testLine = new MathLine(new Vector3(0, 1, 0), new Vector3(5, 1, 0));

        Wheels = new List<Transform>();
        PutAllWheelsIntoAList(ref Wheels);
        #region neighbourWheels
        NeighbouringWheels = new Transform[Wheels.Count, 2];
        Transform[] tempArray;

        for (int i = 0; i < Wheels.Count; i++)
        {
            tempArray = GetClosestWheels(Wheels[i]);
            NeighbouringWheels[i, 0] = tempArray[0];
            NeighbouringWheels[i, 1] = tempArray[1];

            if (!hasReturnRollers)
            {
                if (Wheels[i].GetComponent<Wheel>().WheelType == WheelType.Sprocket)
                {
                    NeighbouringWheels[i, 1] = Idler;
                }
                else if (Wheels[i].GetComponent<Wheel>().WheelType == WheelType.Idler)
                {
                    NeighbouringWheels[i, 1] = Sprocket;
                }
            }
            //Debug.Log(@$"Track Wrapper - {transform.name} - {Wheels[i].name} ({Wheels[i].parent.name}): {NeighbouringWheels[i,0].name} ({NeighbouringWheels[i, 0].parent.name}), {NeighbouringWheels[i,1].name} ({NeighbouringWheels[i, 1].parent.name})");
        }
        #endregion

        SortWheels();

        ConnectingLines = new List<MathLine>();
        Transform currentWheel = null, previousWheel = null;//wheelA, wheelB;
        for(int i = 0; i < Wheels.Count; i++)
        {
            previousWheel = currentWheel;
            currentWheel = Wheels[i];
            if (previousWheel != null)
            {
                MathLine newLine = new MathLine(previousWheel.position,currentWheel.position);
                ConnectingLines.Add(newLine);
            }
        }
        DrawLines(ConnectingLines.ToArray(), ref LineRenderer);
    }

    private void DrawLines(MathLine[] inputLines, ref LineRenderer lineRenderer)
    {
        Vector3[] LinePositions = new Vector3[inputLines.Length * 2];

        for (int i = 0; i < inputLines.Length; i++)
        {
            
            LinePositions[i * 2] = ConnectingLines[i].pointA;
            LinePositions[(i * 2) + 1] = ConnectingLines[i].pointB;
        }
        lineRenderer.positionCount = LinePositions.Length;
        lineRenderer.SetPositions(LinePositions);
    }

    void SortWheels()
    {
        int currIndex = 0;  //The index of the current wheel
        int loopIndex = 1;  //Index of the loop, starts from 1 because the first wheel is skipped.
        Transform tempWheel;
        Transform[] tempWheels = new Transform[2];
        Transform currWheel = Wheels[currIndex];
        Transform nextWheel = NeighbouringWheels[currIndex, 0];
        Transform lastWheel;
        Transform endWheel = NeighbouringWheels[currIndex, 1];
        Debug.Log("Track Wrapper - " + transform.name + " - " + currWheel.name + " - " + currWheel.parent.name);
        while (currWheel != endWheel)
        {
            //Loop mechanism
            lastWheel = currWheel;
            currWheel = nextWheel;
            currIndex = Wheels.IndexOf(currWheel);
            nextWheel = NeighbouringWheels[currIndex, 0] == lastWheel ? NeighbouringWheels[currIndex, 1] : NeighbouringWheels[currIndex, 0];

            //Swapping mechanism
            //Cache the original wheel
            tempWheel = Wheels[loopIndex];
            tempWheels[0] = NeighbouringWheels[loopIndex, 0];
            tempWheels[1] = NeighbouringWheels[loopIndex, 1];
            //Replace the original
            Wheels[loopIndex] = currWheel;
            NeighbouringWheels[loopIndex, 0] = NeighbouringWheels[currIndex,0];
            NeighbouringWheels[loopIndex, 1] = NeighbouringWheels[currIndex,1];
            //Put the cached wheel into the place of the current wheel
            Wheels[currIndex] = tempWheel;
            NeighbouringWheels[currIndex, 0] = tempWheels[0];
            NeighbouringWheels[currIndex, 1] = tempWheels[1];
            
            Debug.Log("Track Wrapper - " + transform.name + " - " + currWheel.name + " (" + currWheel.parent.name + ")");
            loopIndex++;
        }
    }
    void PutAllWheelsIntoAList(ref List<Transform> TargetList)  //Takes the assigned wheel transforms and puts them into a single list
    {
        TargetList.Add(Idler);
        TargetList.Add(Sprocket);
        if (RoadWheelParent != null)
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
        
        //TODO: Move the special condition for sprocket and idler without return rollers into here

        float minDist = float.MaxValue;         //Distance of the closest wheel
        float minDist2 = float.MaxValue;        //Distance of the second closest wheel
        float tempDist;
        Vector3 wheelPos = referenceWheel.position;
            
        foreach(Transform NewWheel in Wheels)
        {
            if (AreWheelsConnectable(referenceWheel.GetComponent<Wheel>(), NewWheel.GetComponent<Wheel>()))
            {
                //The new wheel and the input wheel can theoretically have track between them
                tempDist = Vector3.Distance(wheelPos, NewWheel.position);

                //Runs on the first iteration
                if (ClosestWheels[0] == null)
                {
                    //Since the new wheel is the first one found, it is also the closest one
                    ClosestWheels[0] = NewWheel;
                    minDist = tempDist;     //Distance between the closest wheel and the input wheel
                    continue;
                }
                //Runs on the second iteration
                if (ClosestWheels[1] == null)
                {
                    if (tempDist < minDist)
                    {
                        //The new wheel is closer than the previous one
                        //Set the previously closest wheel to now be the second closest one
                        ClosestWheels[1] = ClosestWheels[0];
                        ClosestWheels[0] = NewWheel;
                        minDist2 = minDist;
                        minDist = tempDist;
                    }
                    else
                    {
                        //The new wheel is further than the previous one
                        //Set the new wheel as the second closest one
                        ClosestWheels[1] = NewWheel;
                        minDist2 = tempDist;
                    }
                    continue;
                }

                if (tempDist < minDist)
                {
                    //New wheel is closer than the closest wheel
                    ClosestWheels[1] = ClosestWheels[0];
                    ClosestWheels[0] = NewWheel;
                    minDist2 = minDist;
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
            return false;
        }
        switch (firstWheel.WheelType)
        {
            //Ordered by the probability of the wheel being the corresponding type (highest to lowest)
            case WheelType.Roadwheel:
                result = secondWheel.WheelType != WheelType.ReturnRoller;
                break;
            case WheelType.ReturnRoller:
                result = secondWheel.WheelType != WheelType.Roadwheel;
                break;
            case WheelType.Sprocket:
                //Doesn't handle the special case where there are no return rollers, this is handled elsewhere
                result = secondWheel.WheelType != WheelType.Idler && secondWheel.WheelType != WheelType.Sprocket;
                break;
            case WheelType.Idler:
                //Doesn't handle the special case where there are no return rollers, this is handled elsewhere
                result = secondWheel.WheelType != WheelType.Sprocket && secondWheel.WheelType != WheelType.Idler;
                break;
        }
        return result;
    }
}
