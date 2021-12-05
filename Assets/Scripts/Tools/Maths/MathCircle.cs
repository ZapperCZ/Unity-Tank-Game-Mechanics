using UnityEngine;

public class MathCircle : MathShape
{
    public float radius { get; set; }
    public MathCircle(Vector3 _pointA, float _radius) : base (_pointA)
    {
        radius = _radius;
    }
    public MathCircle() : base()
    {
        radius = 0;
    }
    public override string ToString()
    {
        return this.GetType().Name + " - Center: " + pointA.ToString() + " Radius: " + radius.ToString();
    }
}
