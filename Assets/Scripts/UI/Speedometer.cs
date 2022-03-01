using System;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public static Speedometer Instance { get; private set; }
    [SerializeField] GameObject SpeedometerUI;
    [SerializeField] float updateInterval = 0.1f;
    public Transform Vehicle;
    float speed;
    float timer = 0;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void OnEnable()
    {
        SpeedometerUI.SetActive(true);
    }
    void OnDisable()
    {
        SpeedometerUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > updateInterval)
        {
            speed = (float) Math.Round(Vehicle.GetComponent<Rigidbody>().velocity.magnitude * 3.6f, 0f);                //Velocity seems to be in m/s, multiplying by 3.6 to convert into km/h
            timer = 0;
            return;
        }
        timer += Time.unscaledTime;
    }
    private void OnGUI()
    {
        SpeedometerUI.GetComponent<UnityEngine.UI.Text>().text = "KM/H: " + speed;
    }
}
