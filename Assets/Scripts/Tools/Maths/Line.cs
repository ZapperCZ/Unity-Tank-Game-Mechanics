using UnityEngine;

public class Line : Shape
{
    public Vector3 pointB { get; set; }
    public Line(Vector3 _pointA, Vector3 _pointB) : base(_pointA)  //posX and posY are the starting point coords
    {
        pointB = _pointB;
    }
    public Line() : base()
    {
        pointB = new Vector3(0,0,0);
    }
}
