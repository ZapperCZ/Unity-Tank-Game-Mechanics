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
    public override string ToString()
    {
        return this.GetType().Name + " - Center: " + pointA.ToString() + " Radius: " + radius.ToString();
    }
}
