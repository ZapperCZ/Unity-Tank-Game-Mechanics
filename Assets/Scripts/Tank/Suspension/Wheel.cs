using UnityEngine;

[ExecuteInEditMode]
public class Wheel : MonoBehaviour
{
    public enum TypeOfWheel
    {
        Sprocket,
        Idler,
        Roadwheel,
        ReturnRoller
    }

    //[SerializeField] float distanceFromAxle = 5f;
    //[SerializeField] Transform WheelAxle;
    [SerializeField] public TypeOfWheel WheelType;

}
