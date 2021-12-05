using UnityEngine;

public class MathShape
{
    public Vector3 pointA { get; set; }
    public MathShape(Vector3 _pointA)
    {
        pointA = _pointA;
    }
    public MathShape()
    {
        pointA = new Vector3(0, 0, 0);
    }
    public override string ToString()
    {
        return this.GetType().Name + " - Position: " + pointA.ToString();
    }
}
