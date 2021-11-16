public class Circle : Shape
{
    public float radius { get; set; }
    public Circle(float _posX, float _posY, float _radius) : base (_posX, _posY)
    {
        radius = _radius;
    }
    public Circle() : base()
    {
        radius = 0;
    }
}
