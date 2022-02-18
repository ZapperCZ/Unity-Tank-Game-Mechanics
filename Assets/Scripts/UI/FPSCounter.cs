using System;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] GameObject CounterUI;
    [SerializeField] float updateInterval = 0.5f; //How often should fps update in seconds
    float fps = 0;
    float counter = 0;

    // Update is called once per frame
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
