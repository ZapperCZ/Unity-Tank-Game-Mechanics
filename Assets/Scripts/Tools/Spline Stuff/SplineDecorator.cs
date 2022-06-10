using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class SplineDecorator : MonoBehaviour
{
	[SerializeField] bool linearSpacing = false;
	[Header("Linear Settings")]
	[SerializeField] int linearizationPrecision = 100;
	[SerializeField] float itemSpacing = 0.5f;
	[Header("Non-Linear settings")]
	[SerializeField] int frequency;
	[Header("Misc")]
	[SerializeField] bool lookForward;
	[SerializeField] BezierSpline spline;
	[SerializeField] Transform inputItem;

	bool inputChanged = false;
	float[] linearizedArray;

	private void Awake()
	{
		DeleteObjects();
		if (linearSpacing)
			GenerateObjectsLinearly();
		else
			GenerateObjects();
	}
    private void OnValidate()
    {
		inputChanged = true;
    }
    private void Update()
    {
        if (inputChanged)
        {
			DeleteObjects();
			if (linearSpacing)
				GenerateObjectsLinearly();
			else
				GenerateObjects();
			inputChanged = false;
        }
    }
	private void DeleteObjects()
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
	private float[] LinearizeSpline(BezierSpline inputSpline, int precision, float spacing)	//Returns an array of points along the input spline, higher precision >> more points
    {
		int steps = precision * inputSpline.CurveCount;
		List<float> resultList = new List<float>();
		Vector3 lastPosition = spline.GetPoint(linearizedArray[0]);
		Vector3 currentPosition = spline.GetPoint(linearizedArray[0]);
		float distance = 0;

		for (int i = 1; i <= steps; i++)
		{
			//result[i] = i / (float)steps;
			Debug.Log(i);
		}
		float[] result = resultList.ToArray();
		return result;
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
		if (itemSpacing < 0 || inputItem == null)
			return;

		linearizedArray = LinearizeSpline(spline, linearizationPrecision, itemSpacing);

		Vector3 lastPosition = spline.GetPoint(linearizedArray[0]);
		Vector3 currentPosition = spline.GetPoint(linearizedArray[0]);
		float distance = 0;
		Transform item;

		item = Instantiate(inputItem) as Transform;
		item.transform.localPosition = lastPosition;
		//item.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
		if (lookForward)
		{
			item.transform.LookAt(lastPosition + spline.GetDirection(linearizedArray[0]));
		}
		item.transform.parent = transform;

		for (int i = 1; i < linearizedArray.Length; i++)
		{
			lastPosition = currentPosition;
			currentPosition = spline.GetPoint(linearizedArray[i]);
			distance += Vector3.Distance(lastPosition, currentPosition);
			if(distance >= itemSpacing)
            {
				item = Instantiate(inputItem) as Transform;
				item.transform.localPosition = currentPosition;
				if (lookForward)
				{
					item.transform.LookAt(lastPosition + spline.GetDirection(linearizedArray[i]));
				}
				item.transform.parent = transform;
				distance = 0;
			}
		}
		//item.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
	}
}