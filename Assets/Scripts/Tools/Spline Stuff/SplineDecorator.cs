using UnityEngine;

[ExecuteInEditMode]

public class SplineDecorator : MonoBehaviour
{

	private const int stepsPerCurve = 10;
	public BezierSpline spline;
	public int frequency;
	public bool lookForward;
	public Transform inputItem;
	bool inputChanged = false;

	private void Awake()
	{
		DeleteObjects();
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
	private void GenerateObjects()
    {
		if (frequency <= 0 || inputItem == null)
		{
			return;
		}
		float stepSize = frequency;
		if (spline.Loop || stepSize == 1)
		{
			stepSize = 1f / stepSize;
		}
		else
		{
			stepSize = 1f / (stepSize - 1);
		}

		//Vector3 point = spline.GetPoint(0f);
		Vector3 position = spline.GetPoint(0f);
		Transform item;
		//Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
		int steps = frequency * spline.CurveCount;
		for (int i = 1; i <= steps; i++)
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
}