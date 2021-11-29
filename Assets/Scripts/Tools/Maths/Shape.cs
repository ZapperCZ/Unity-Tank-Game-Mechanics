using UnityEngine;

public class Shape
{
    public Vector3 pointA { get; set; }
    public Shape(Vector3 _pointA)
    {
        pointA = _pointA;
    }
    public Shape()
    {
        pointA = new Vector3(0, 0, 0);
    }
    public override string ToString()
    {
        return this.GetType().Name + " - Position: " + pointA.ToString();
    }
}
