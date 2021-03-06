using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class SplineDecorator : MonoBehaviour
{
	[SerializeField] bool linearSpacing = false;			//Whether linear spacing should be enabled or not
	[Header("Linear Settings")]
	[SerializeField] int linearizationPrecision = 1000;		//The accuracy of spline linearization. Higher number >> Higher precision
	[SerializeField] float itemSpacing = 0.5f;              //The linear spacing between items
	[SerializeField] bool scaleItemsToFitAll = true;		//Toggles the spacing and scaling of all items in order to properly space the first and last item
															//Enabling this option increases computational complexity by a factor of 2
	[Header("Non-Linear settings")]
	[SerializeField] int frequency = 10;					//The frequency of non-linear item generation
	[Header("Misc")]
	[SerializeField] bool lookForward = true;				//Whether the items should point along the spline or not
	[SerializeField] BezierSpline spline;					//The spline to be decorated
	[SerializeField] Transform inputItem;					//The item for the spline to be decorated with
	[SerializeField] bool debugConsoleOutput = false;       //Whether the debug info should be output to console or not
	[SerializeField] bool debugColorFirstAndLastItem = false;	//Whether the first and last items should be distinguished by color from others

	//------------ Internal properties ------------//
	bool inputChanged = false;	//Detects an interaction of the user with values accessible through the Inspector window
	float[] linearizedArray;    //Stores points along the spline with linear spacing between them
	List<float> debug_SpacingDeviationList;	//List of all spacing deviations
	float debug_LastToFirstItemDistance;    //Distance between the last and first item
	float debug_ActualSpacing;

	private void Awake()
	{
		DeleteObjects();
		if (linearSpacing)
			GenerateObjectsLinearly();
		else
			GenerateObjects();
	}
    private void OnValidate()	//Usually gets called when an Inspector accessible variable has been changed
    {
		inputChanged = true;
    }
    private void Update()
    {
        if (inputChanged)	//Only regenerate the objects when the values have been updated - most probably doesn't work in the build version
        {
			DeleteObjects();
			if (linearSpacing)
				GenerateObjectsLinearly();
			else
				GenerateObjects();
			inputChanged = false;
        }
    }
	private void DeleteObjects()	//Safely disposes of all current child transforms
    {
		int childCount = transform.childCount;
		Transform child = null;
		for (int i = 0; i < childCount; i++)	//Iterate though all children transforms
		{
			if (Application.isPlaying)
			{
				child = transform.GetChild(i);
			}
			else if (Application.isEditor)
			{
				child = transform.GetChild(0);
			}
			DestroyObjectSafely(child.gameObject);
		}
    }
	void DestroyObjectSafely(Object obj)
	{
		if (Application.isPlaying)
		{
			Destroy(obj);
		}
		else if (Application.isEditor)
		{
			DestroyImmediate(obj);
		}
	}
	private float[] LinearizeSpline(BezierSpline inputSpline, int precision, float spacing)	//Returns an array of linearly spaced points along the input spline, higher precision >> more points
    {
		debug_SpacingDeviationList = new List<float>();
		List<float> resultList = new List<float>();     //List of linearly spaced points on the spline. Is a List because the amount of points isn't predictable
		int steps = precision * inputSpline.CurveCount;	//Amount of steps, ensures that precision stays the same, independent from the amount of curves on the spline
		Vector3 lastPosition;							
		Vector3 currentPosition = spline.GetPoint(0);	
		float tempCalc;									//A temporary calculation that gets saved into the memory to offload work from the CPU
		float distance = spacing;                       //Distance between current and last position, increases with each iteration until it is larger than spacing value
		float prevDistance;

		for (int i = 0; i <= steps; i++)	//Point generation
		{
			tempCalc = i / (float)steps;
			lastPosition = currentPosition;
			currentPosition = spline.GetPoint(tempCalc);
			prevDistance = distance;
			distance += Vector3.Distance(lastPosition, currentPosition);	//Adds distance between current and last position

			if (distance >= spacing)    //Current distance is equal or more than the desired item spacing > current point is desired
			{
				if(Mathf.Abs(spacing - prevDistance) < Mathf.Abs(spacing - distance))	//Previous point was closer than the current point - previous point will be saved
				{
					resultList.Add((i - 1) / (float)steps);                 //Using the previous position on the spline instead of the current one
					debug_SpacingDeviationList.Add(spacing - prevDistance);
				}
				else																	//Current point is closer than the previous point - current point will be saved
				{
					resultList.Add(tempCalc);
					debug_SpacingDeviationList.Add(distance - spacing);
				}
				distance = 0;
            }
		}

		debug_LastToFirstItemDistance = distance;

		if (scaleItemsToFitAll)
        {
			spacing = spacing - ((spacing - debug_LastToFirstItemDistance) / (resultList.Count + 1));   //Get the distance that is missing between the first and last item
																										//distribute it over all items and make the result spacing the default one
																										//FIX: Distance between first and last item can be 0 after the recalculation rendering it pointless
			//Re-instantiate filled lists
			debug_SpacingDeviationList = new List<float>();
			resultList = new List<float>();

			//Repeat point generation
			currentPosition = spline.GetPoint(0);
			distance = spacing;
			for (int i = 0; i <= steps; i++)
			{
				tempCalc = i / (float)steps;
				lastPosition = currentPosition;
				currentPosition = spline.GetPoint(tempCalc);
				prevDistance = distance;
				distance += Vector3.Distance(lastPosition, currentPosition);
				if (distance >= spacing)
				{
					if (Mathf.Abs(spacing - prevDistance) < Mathf.Abs(spacing - distance))  //Previous point was closer than the current point - previous point will be saved
					{
						resultList.Add((i - 1) / (float)steps);					//Using the previous position on the spline instead of the current one
						debug_SpacingDeviationList.Add(spacing - prevDistance);
					}
					else                                                                    //Current point is closer than the previous point - current point will be saved
					{
						resultList.Add(tempCalc);
						debug_SpacingDeviationList.Add(distance - spacing);
					}
					distance = 0;
				}
			}
			if (distance < spacing / 2)
            {
				resultList.Remove(resultList[resultList.Count-1]);
            }
			debug_LastToFirstItemDistance = distance;
		}
		debug_ActualSpacing = spacing;
		return resultList.ToArray();	//Returns an array for memory optimalization purposes
    }
	void GenerateObjects()	//Decorate spline with spacing based on item frequency
    {
		if (frequency <= 0 || inputItem == null)	//Prevents input of undesired values
			return;
		float stepSize = frequency;	
		if(spline.Loop || stepSize == 1)
        {
			stepSize = 1f / stepSize;
        }
        else
        {
			stepSize = 1f / (stepSize - 1);
        }

		Vector3 position;
		Transform item;
		int steps = frequency * spline.CurveCount;
		for(int i = 1; i<= steps; i++)
        {
			position = spline.GetPoint(i / (float)steps);
			item = Instantiate(inputItem) as Transform;
			item.transform.localPosition = position;
            if (lookForward)
            {
				item.transform.LookAt(position + spline.GetDirection(i / (float)steps));
            }
			item.transform.parent = transform;
			if (debugColorFirstAndLastItem)
			{
				if (i == 1) //First item
				{
					Renderer itemRenderer = item.GetComponent<Renderer>();
					itemRenderer.material = new Material(inputItem.GetComponent<Renderer>().sharedMaterial.shader);   //Create a new material for the item so that the sharedMaterial can be used to avoid instantiating new materials unexpectadly
					itemRenderer.sharedMaterial.color = Color.green;
				}
				if (i == steps)    //Last item
				{
					Renderer itemRenderer = item.GetComponent<Renderer>();
					itemRenderer.material = new Material(inputItem.GetComponent<Renderer>().sharedMaterial.shader);
					itemRenderer.sharedMaterial.color = Color.red;
				}
			}
		}

    }
	void GenerateObjectsLinearly()	//Decorate spline with linear spacing between items
    {
		if (itemSpacing <= 0 || linearizationPrecision <= 0 || inputItem == null)	//Prevents input of undesired values
			return;

		linearizedArray = LinearizeSpline(spline, linearizationPrecision, itemSpacing);
        if (debugConsoleOutput)
        {
			int pointAmount = linearizedArray.Length;
			float minDeviation = float.MaxValue;
			float maxDeviation = 0f;
			float deviationSum = 0f;
			for(int i = 1; i < debug_SpacingDeviationList.Count; i++)	//Starts from second element to avoid polluting the data with the first element which always gets generated properly
			{
				float f = debug_SpacingDeviationList[i];
				minDeviation = f < minDeviation ? f : minDeviation;
				maxDeviation = f > maxDeviation ? f : maxDeviation;
				deviationSum += f;
			}
			float averageDeviation = deviationSum / (float)debug_SpacingDeviationList.Count;
			Debug.Log("Amount of points >> " + pointAmount);
			Debug.Log("Actual spacing used >> " + debug_ActualSpacing);
			Debug.Log("Minimum spacing deviation >> " + minDeviation);
			Debug.Log("Maximum spacing deviation >> " + maxDeviation);
			Debug.Log("Average deviation >> " + averageDeviation);
			Debug.Log("Distance from last to first item >> " + debug_LastToFirstItemDistance);
		}	//Debug output logic
		Vector3 position;	//Position of the item that is currently being generated
		Transform item;		//Item to populate the spline with

		for (int i = 0; i < linearizedArray.Length; i++)	//Iterate through all the linearly spaced points
		{
			position = spline.GetPoint(linearizedArray[i]);
			item = Instantiate(inputItem) as Transform;
			item.transform.localPosition = position;
			if (lookForward)
			{
				item.transform.LookAt(position + spline.GetDirection(linearizedArray[i]));	//Look in the direction of the spline at the current position
			}
			item.transform.parent = transform;	//Set the decorator as the item parent
			if(debugColorFirstAndLastItem)
            {
				if (i == 0)	//First item
				{
					Renderer itemRenderer = item.GetComponent<Renderer>();
					itemRenderer.material = new Material(inputItem.GetComponent<Renderer>().sharedMaterial.shader);	//Create a new material for the item so that the sharedMaterial can be used to avoid instantiating new materials unexpectadly
					itemRenderer.sharedMaterial.color = Color.green;
				}
                if (i == linearizedArray.Length - 1)	//Last item
                {
					Renderer itemRenderer = item.GetComponent<Renderer>();
					itemRenderer.material = new Material(inputItem.GetComponent<Renderer>().sharedMaterial.shader);
					itemRenderer.sharedMaterial.color = Color.red;
				}
            }
		}
	}
}