using UnityEngine;

public class MathLine : MathShape
{
    public Vector3 pointB { get; set; }
    public MathLine(Vector3 _pointA, Vector3 _pointB) : base(_pointA)  //posX and posY are the starting point coords
    {
        pointB = _pointB;
    }
    public MathLine() : base()
    {
        pointB = new Vector3(0,0,0);
    }
    public override string ToString()
    {
        return this.GetType().Name +  " - Position: " + pointA.ToString() + " End: " + pointB.ToString();
    }
}
