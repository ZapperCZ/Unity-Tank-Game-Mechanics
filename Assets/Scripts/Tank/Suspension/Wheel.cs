using UnityEngine;

[ExecuteInEditMode]
public class Wheel : MonoBehaviour
{
    enum TypeOfWheel
    {
        Sprocket,
        Idler,
        Roadwheel,
        ReturnRoller
    }

    //[SerializeField] float distanceFromAxle = 5f;
    //[SerializeField] Transform WheelAxle;
    [SerializeField] TypeOfWheel WheelType;

}
