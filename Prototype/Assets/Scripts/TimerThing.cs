using UnityEngine;
using TMPro;
using System;
using System.Diagnostics;
public class TimerThing : MonoBehaviour
{
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);

        }
        instance = this;
    }
    public TMP_Text timerText;
    public Stopwatch timer;
    public static TimerThing instance;

    void Start()
    {
        timer = new Stopwatch();
        timer.Restart();
        timer.Start();
        DontDestroyOnLoad(gameObject);
    }

    public void StopTimer()
    {
        timer.Stop();

    }

    void Update()
    {
        if (timer.IsRunning)
        {
            int minutes = (int)timer.Elapsed.TotalMinutes;
            int seconds = (int)timer.Elapsed.TotalSeconds % 60;
            int milliseconds = (int)timer.ElapsedMilliseconds % 1000;

            timerText.text = $"{minutes:D2}:{seconds:D2}:{milliseconds:D2}";
        }
    }
}
