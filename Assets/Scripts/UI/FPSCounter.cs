using System;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] GameObject CounterUI;
    [SerializeField] float updateInterval = 0.5f; //How often should fps update in seconds
    [SerializeField] int fpsLimit = 60;
    float fps = 0;
    float counter = 0;
    private void Start()
    {
        Application.targetFrameRate = fpsLimit;
    }
    void OnValidate()
    {
        Application.targetFrameRate = fpsLimit;
    }
    void Update()
    {
        if(counter > updateInterval)
        {
            fps = 1 / Time.unscaledDeltaTime;
            counter = 0;
            return;
        }   
        counter += Time.unscaledDeltaTime;
    }
    void OnGUI()
    {
        CounterUI.GetComponent<UnityEngine.UI.Text>().text = Math.Round(fps,0).ToString() + " FPS";
    }
}
