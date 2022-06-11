using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class SplineDecorator : MonoBehaviour
{
	[SerializeField] bool linearSpacing = false;			//Whether linear spacing should be enabled or not
	[Header("Linear Settings")]
	[SerializeField] int linearizationPrecision = 1000;		//The accuracy of spline linearization. Higher number >> Higher precision
	[SerializeField] float itemSpacing = 0.5f;				//The linear spacing between items
	[Header("Non-Linear settings")]
	[SerializeField] int frequency = 10;					//The frequency of non-linear item generation
	[Header("Misc")]
	[SerializeField] bool lookForward = true;				//Whether the items should point along the spline or not
	[SerializeField] BezierSpline spline;					//The spline to be decorated
	[SerializeField] Transform inputItem;					//The item for the spline to be decorated with

	//------------ Internal properties ------------//
	bool inputChanged = false;	//Detects an interaction of the user with values accessible through the Inspector window
	float[] linearizedArray;	//Stores points along the spline with linear spacing between them

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
		for (int i = 0; i < childCount; i++)
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
		int steps = precision * inputSpline.CurveCount;	//Better tuned precision value, ensures that precision stays the same no matter the size of the spline
		List<float> resultList = new List<float>();		
		Vector3 lastPosition;
		Vector3 currentPosition = spline.GetPoint(0);
		float calc;
		float distance = spacing;

		for (int i = 0; i <= steps; i++)
		{
			calc = i / (float)steps;
			lastPosition = currentPosition;
			currentPosition = spline.GetPoint(calc);
			distance += Vector3.Distance(lastPosition, currentPosition);
			if(distance >= spacing)
            {
				resultList.Add(calc);
				distance = 0;
            }
		}
		return resultList.ToArray();	//Returns an array for memory optimalization purposes
    }
	void GenerateObjects()
    {
		if (frequency <= 0 || inputItem == null)
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

		Vector3 position = spline.GetPoint(0f);
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
        }

    }
	void GenerateObjectsLinearly()
    {
		if (itemSpacing <= 0 || inputItem == null)
			return;

		linearizedArray = LinearizeSpline(spline, linearizationPrecision, itemSpacing);

		Vector3 position;
		Transform item;

		for (int i = 0; i < linearizedArray.Length; i++)
		{
			position = spline.GetPoint(linearizedArray[i]);
			item = Instantiate(inputItem) as Transform;
			item.transform.localPosition = position;
			if (lookForward)
			{
				item.transform.LookAt(position + spline.GetDirection(linearizedArray[i]));
			}
			item.transform.parent = transform;
		}
	}
}