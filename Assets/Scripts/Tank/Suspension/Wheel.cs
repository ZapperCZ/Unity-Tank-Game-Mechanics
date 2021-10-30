using UnityEngine;

[ExecuteInEditMode]
public class Wheel : MonoBehaviour
{
    [SerializeField] float distanceFromAxle = 5f;
    [SerializeField] Transform WheelAxle;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnValidate()
    {
        if (!Application.isPlaying)
        {

        }
    }
    void Update()
    {
        
    }
}
