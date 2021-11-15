public class Line : Shape
{
    public float endPosX { get; set; }
    public float endPosY { get; set; }
    public Line(float _posX, float _posY, float _endPosX, float _endPosY) : base(_posX, _posY)  //posX and posY are the starting point coords
    {
        endPosX = _endPosX;
        endPosY = _endPosY;
    }
}
