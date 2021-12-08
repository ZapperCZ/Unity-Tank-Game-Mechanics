using UnityEngine;

[ExecuteInEditMode]

public class SplineDecorator : MonoBehaviour
{

	public BezierSpline spline;

	public int frequency;

	public bool lookForward;

	public Transform[] items;
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
		if (frequency <= 0 || items == null || items.Length == 0)
		{
			return;
		}
		float stepSize = frequency * items.Length;
		if (spline.Loop || stepSize == 1)
		{
			stepSize = 1f / stepSize;
		}
		else
		{
			stepSize = 1f / (stepSize - 1);
		}
		for (int p = 0, f = 0; f < frequency; f++)
		{
			for (int i = 0; i < items.Length; i++, p++)
			{
				Transform item = Instantiate(items[i]) as Transform;
				Vector3 position = spline.GetPoint(p * stepSize);
				item.transform.localPosition = position;
				if (lookForward)
				{
					item.transform.LookAt(position + spline.GetDirection(p * stepSize));
				}
				item.transform.parent = transform;
			}
		}
	}
}