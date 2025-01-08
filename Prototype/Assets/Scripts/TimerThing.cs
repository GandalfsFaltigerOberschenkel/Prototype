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

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log(timer.ElapsedMilliseconds);
        timerText.text = $"{Mathf.Round((float)timer.Elapsed.TotalMinutes)}:{Mathf.Round((float)timer.Elapsed.TotalSeconds) % 60}:{Mathf.Round(timer.ElapsedMilliseconds % 1000)}";
    }
}
