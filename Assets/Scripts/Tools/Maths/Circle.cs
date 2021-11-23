using UnityEngine;

public class Circle : Shape
{
    public float radius { get; set; }
    public Circle(Vector3 _pointA, float _radius) : base (_pointA)
    {
        radius = _radius;
    }
    public Circle() : base()
    {
        radius = 0;
    }
}
