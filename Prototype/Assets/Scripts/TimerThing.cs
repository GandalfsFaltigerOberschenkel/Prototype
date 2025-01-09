using UnityEngine;
using TMPro;
using System;
using System.Diagnostics;
public class TimerThing : MonoBehaviour
{
    public TMP_Text timerText;
    public System.Diagnostics.Stopwatch timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = new Stopwatch();
        timer.Restart();
        timer.Start();
    }
    public void StopTimer()
    {

        timer.Stop();
    }
    

    // Update is called once per frame
    void Update()
    {
        timerText.text = $"{Mathf.Round((float)timer.Elapsed.TotalMinutes)}:{Mathf.Round((float)timer.Elapsed.TotalSeconds) % 60}:{Mathf.Round(timer.ElapsedMilliseconds % 1000)}";
    }
}
